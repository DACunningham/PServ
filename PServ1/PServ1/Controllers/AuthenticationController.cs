using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace PServ1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private const string _clientId = "2lkl9n6iuicb798bkkh7rria48";
        private readonly RegionEndpoint _region = RegionEndpoint.USWest2;

        public class User
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string Email { get; set; }
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<string>> Register(User user)
        {
            var cognito = new AmazonCognitoIdentityProviderClient(_region);

            var request = new SignUpRequest
            {
                ClientId = _clientId,
                Password = user.Password,
                Username = user.Username
            };

            var emailAttribute = new AttributeType
            {
                Name = "email",
                Value = user.Email
            };
            request.UserAttributes.Add(emailAttribute);

            var response = await cognito.SignUpAsync(request);

            var groupResponse = await cognito.AdminAddUserToGroupAsync(new AdminAddUserToGroupRequest
            {
                GroupName = "Admin",
                Username = user.Username,
                UserPoolId = "us-west-2_TQaxeubl4"
            });

            var confResponse = await cognito.AdminConfirmSignUpAsync(new AdminConfirmSignUpRequest
            {
                Username = user.Username,
                UserPoolId = "us-west-2_TQaxeubl4"
            });

            return Ok();
        }

        [HttpPost]
        [Route("signin")]
        public async Task<ActionResult<string>> SignIn(User user)
        {
            var cognito = new AmazonCognitoIdentityProviderClient(_region);

            var request = new AdminInitiateAuthRequest
            {
                UserPoolId = "us-west-2_TQaxeubl4",
                ClientId = _clientId,
                AuthFlow = AuthFlowType.ADMIN_NO_SRP_AUTH
            };

            request.AuthParameters.Add("USERNAME", user.Username);
            request.AuthParameters.Add("PASSWORD", user.Password);

            var response = await cognito.AdminInitiateAuthAsync(request);

            return Ok(response.AuthenticationResult.IdToken);
        }

        [HttpPost]
        [Route("signout")]
        public async Task<ActionResult<string>> SignOut(User user)
        {
            var cognito = new AmazonCognitoIdentityProviderClient(_region);

            var request = new AdminUserGlobalSignOutRequest();

            request.Username = user.Username;
            request.UserPoolId = "us-west-2_TQaxeubl4";

            //var response = await cognito.AdminInitiateAuthAsync(request);
            var response = await cognito.AdminUserGlobalSignOutAsync(request);

            return Ok(response);
        }
    }

    // A handler that can determine whether a MaximumOfficeNumberRequirement is satisfied
    internal class MaximumOfficeNumberAuthorizationHandler : AuthorizationHandler<MaximumOfficeNumberRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MaximumOfficeNumberRequirement requirement)
        {
            // Bail out if the office number claim isn't present
            if (!context.User.HasClaim(c => c.Issuer == "https://cognito-idp.us-west-2.amazonaws.com/us-west-2_TQaxeubl4" && c.Type == "cognito:groups"))
            {
                return Task.CompletedTask;
            }

            // Bail out if we can't read an int from the 'office' claim
            string officeNumber = context.User.FindFirst(c => c.Issuer == "https://cognito-idp.us-west-2.amazonaws.com/us-west-2_TQaxeubl4" && c.Type == "cognito:groups").Value;
            if (officeNumber == null)
            {
                return Task.CompletedTask;
            }

            // Finally, validate that the office number from the claim is not greater
            // than the requirement's maximum
            if (officeNumber == requirement.MaximumOfficeNumber)
            {
                // Mark the requirement as satisfied
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    // A custom authorization requirement which requires office number to be below a certain value
    internal class MaximumOfficeNumberRequirement : IAuthorizationRequirement
    {
        public MaximumOfficeNumberRequirement(string role)
        {
            MaximumOfficeNumber = role;
        }

        public string MaximumOfficeNumber { get; private set; }
    }
}
