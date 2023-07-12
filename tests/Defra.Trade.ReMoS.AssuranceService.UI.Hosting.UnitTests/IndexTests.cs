using Defra.ReMoS.AssuranceService.UI.Hosting.Pages;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Configuration;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Establishments;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests
{
    [TestFixture]
    public class IndexTests
    {
        private IndexModel? _systemUnderTest;
        protected Mock<ILogger<IndexModel>> _mockLogger = new();
        protected Mock<IOptions<EhcoIntegrationSettings>> _mockIntegrationSettings = new();

        [SetUp]
        public void TestCaseSetup()
        {
            _systemUnderTest = new IndexModel(_mockLogger.Object, _mockIntegrationSettings.Object);
        }

        [Test]
        public void DecodeJwt_Success()
        {
            var jwt = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJTb21lQ2xhaW0iOiJkYXRhIiwibmJmIjoxNjg5MTY4ODg2LCJleHAiOjE2ODkxNzI0ODYsImlzcyI6IkFwcGxpY2F0aW9uMSIsImF1ZCI6InRlc3QxIn0.DHu5ARcSGrcnuy5Z122CrJqT0McVWBkprp67Rb6iavlLeNxTA0f-e-wF7IZhWnj2hRGrzcRTHp8YzgZqNrup-OC6xC4OyxWahfNvb5ZznPm87F_JBNRD6sMaZA0B3EU0MLwmqMA_RWSt8i_tSM6vZqYPQvLFnd-H6ysqSrhBdxhBaalaJuDxKc-5hiHKPbC0Rk3pGr-PdvFIq9sydLxXgSq_lUkAVS81Vl-JxIP1BTbYtoQiyn8AveaI6JB4bubmVrkDW9OeSEOFYDQ_dSebV6VcrasYrA9uQOEBF6L7iCKAFHNbDnt3LsxwWX45ys2jtwTAusAKJBvzaSZ9aKx0AfxwRPO6HvC6X7HKF7EUw3gtSyvrHPwdtdHc259LZ1cLFeMacvDNaRB9vraPVLni_c7g1lJfsdBdhadWZVg_GC1XGWHnZpMERXDwPRzhAXo7kO5SWbQJJv1f9PMn1vaxyIDpbholxqWxKTp6JBLe_LbirTxZwDHZUkj1lIThfRHUzqypwQbwZL_l0lqUVaOcsgTbhCDfzTWHdEN4a2DE1wKxPWLYOXQgBrNYmR0ZVYVrEh2h_llMSosJ55ASuejIVRjSpZSBcBELL2LplM_qZrKpxFpO0qxKr9N78r5tcQhpcRkd0j1HCqvqqOTNICkEMRcNphQYRCOejQsRXIgyAAk";

            var pubKey = "-----BEGIN PUBLIC KEY-----\r\nMIICIjANBgkqhkiG9w0BAQEFAAOCAg8AMIICCgKCAgEAxPl0/QnZMHXp5k8lx9qr\r\n/dcjDa4PLqfD8+nUEPFO+rO29A6cUcwQgSKK5HFyEylAYqiIOH07d6wMz2FGKKPr\r\nc3xHGvJMQVdGdpl693mQWjqlJMkjmq+z309Miqas356wktOAz7VMLhvJ80ScPHC1\r\nxeEivwrMV/pCjD/o6DinLYevHT/yklXfY79Np9cU103jbh6Ax6q4rkbRiqCgaG9H\r\nFIogmMR4w428imyGXcqsU+PWWIjYgvawel37U6DiDxLTBN3Q5w7tyJpi2+JY43Ib\r\n2GJb8VVKGsUynKA0sCCqiNVLPHwy3yTrHdQJLbb/S65HBWEIXM5PwxWwHMZZPwnZ\r\nLnoBz3HH45aciVV/R14crqTASph2z4yr6aRDqL9OOs+Qh8vDhWsdHAn2AHmMycGh\r\nAxO4T9ohqAqLcTzTgWPizQ5sJRtyvj0ByXfvY6wy31iS38zEeS3ILDA5sPVPkJ6a\r\nK46ojDZNiRHWjaCmotih/coxUlUeFkQ+uXPd8Wj23JtPWeAzjdJfSLunHJDe04FX\r\nhWFAeIA0EAzQJG3guN6qrCuHSYl4j3WHV193ymFzQkKgqkqMOpp1JmqEhMUuzeOq\r\n0dlzZXpktuGtJDSFsxzus6//xJ8QVuEq0Uv3TiROPnBA2nq3FsUzXAwcW75Nqhse\r\nXqaPZlTNOKk7fTj70vNuPucCAwEAAQ==\r\n-----END PUBLIC KEY-----\r\n";
            var rsaPublicKey = RSA.Create();
            rsaPublicKey.ImportFromPem(pubKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuer = true,
                ValidAudience = "test1",
                ValidIssuer = "Application1",
                IssuerSigningKey = new RsaSecurityKey(rsaPublicKey)
            };

            var decodedToken = _systemUnderTest?.DecodeJwt(jwt, validationParameters);

            Assert.IsNotNull(decodedToken);
        }

        [Test]        
        public void DecodeJwt_Invalid_Token_InvalidIssuer()
        {
            var jwt = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJTb21lQ2xhaW0iOiJkYXRhIiwibmJmIjoxNjg5MTY4ODg2LCJleHAiOjE2ODkxNzI0ODYsImlzcyI6IkFwcGxpY2F0aW9uMSIsImF1ZCI6InRlc3QxIn0.DHu5ARcSGrcnuy5Z122CrJqT0McVWBkprp67Rb6iavlLeNxTA0f-e-wF7IZhWnj2hRGrzcRTHp8YzgZqNrup-OC6xC4OyxWahfNvb5ZznPm87F_JBNRD6sMaZA0B3EU0MLwmqMA_RWSt8i_tSM6vZqYPQvLFnd-H6ysqSrhBdxhBaalaJuDxKc-5hiHKPbC0Rk3pGr-PdvFIq9sydLxXgSq_lUkAVS81Vl-JxIP1BTbYtoQiyn8AveaI6JB4bubmVrkDW9OeSEOFYDQ_dSebV6VcrasYrA9uQOEBF6L7iCKAFHNbDnt3LsxwWX45ys2jtwTAusAKJBvzaSZ9aKx0AfxwRPO6HvC6X7HKF7EUw3gtSyvrHPwdtdHc259LZ1cLFeMacvDNaRB9vraPVLni_c7g1lJfsdBdhadWZVg_GC1XGWHnZpMERXDwPRzhAXo7kO5SWbQJJv1f9PMn1vaxyIDpbholxqWxKTp6JBLe_LbirTxZwDHZUkj1lIThfRHUzqypwQbwZL_l0lqUVaOcsgTbhCDfzTWHdEN4a2DE1wKxPWLYOXQgBrNYmR0ZVYVrEh2h_llMSosJ55ASuejIVRjSpZSBcBELL2LplM_qZrKpxFpO0qxKr9N78r5tcQhpcRkd0j1HCqvqqOTNICkEMRcNphQYRCOejQsRXIgyAAk";

            var pubKey = "-----BEGIN PUBLIC KEY-----\r\nMIICIjANBgkqhkiG9w0BAQEFAAOCAg8AMIICCgKCAgEAxPl0/QnZMHXp5k8lx9qr\r\n/dcjDa4PLqfD8+nUEPFO+rO29A6cUcwQgSKK5HFyEylAYqiIOH07d6wMz2FGKKPr\r\nc3xHGvJMQVdGdpl693mQWjqlJMkjmq+z309Miqas356wktOAz7VMLhvJ80ScPHC1\r\nxeEivwrMV/pCjD/o6DinLYevHT/yklXfY79Np9cU103jbh6Ax6q4rkbRiqCgaG9H\r\nFIogmMR4w428imyGXcqsU+PWWIjYgvawel37U6DiDxLTBN3Q5w7tyJpi2+JY43Ib\r\n2GJb8VVKGsUynKA0sCCqiNVLPHwy3yTrHdQJLbb/S65HBWEIXM5PwxWwHMZZPwnZ\r\nLnoBz3HH45aciVV/R14crqTASph2z4yr6aRDqL9OOs+Qh8vDhWsdHAn2AHmMycGh\r\nAxO4T9ohqAqLcTzTgWPizQ5sJRtyvj0ByXfvY6wy31iS38zEeS3ILDA5sPVPkJ6a\r\nK46ojDZNiRHWjaCmotih/coxUlUeFkQ+uXPd8Wj23JtPWeAzjdJfSLunHJDe04FX\r\nhWFAeIA0EAzQJG3guN6qrCuHSYl4j3WHV193ymFzQkKgqkqMOpp1JmqEhMUuzeOq\r\n0dlzZXpktuGtJDSFsxzus6//xJ8QVuEq0Uv3TiROPnBA2nq3FsUzXAwcW75Nqhse\r\nXqaPZlTNOKk7fTj70vNuPucCAwEAAQ==\r\n-----END PUBLIC KEY-----\r\n";
            var rsaPublicKey = RSA.Create();
            rsaPublicKey.ImportFromPem(pubKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuer = true,
                ValidAudience = "test1",
                ValidIssuer = "Application2",
                IssuerSigningKey = new RsaSecurityKey(rsaPublicKey)
            };

            Assert.Throws<SecurityTokenInvalidIssuerException>(() => _systemUnderTest?.DecodeJwt(jwt, validationParameters));
        }

        [Test]
        public void DecodeJwt_Invalid_Token_InvalidAudience()
        {
            var jwt = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJTb21lQ2xhaW0iOiJkYXRhIiwibmJmIjoxNjg5MTY4ODg2LCJleHAiOjE2ODkxNzI0ODYsImlzcyI6IkFwcGxpY2F0aW9uMSIsImF1ZCI6InRlc3QxIn0.DHu5ARcSGrcnuy5Z122CrJqT0McVWBkprp67Rb6iavlLeNxTA0f-e-wF7IZhWnj2hRGrzcRTHp8YzgZqNrup-OC6xC4OyxWahfNvb5ZznPm87F_JBNRD6sMaZA0B3EU0MLwmqMA_RWSt8i_tSM6vZqYPQvLFnd-H6ysqSrhBdxhBaalaJuDxKc-5hiHKPbC0Rk3pGr-PdvFIq9sydLxXgSq_lUkAVS81Vl-JxIP1BTbYtoQiyn8AveaI6JB4bubmVrkDW9OeSEOFYDQ_dSebV6VcrasYrA9uQOEBF6L7iCKAFHNbDnt3LsxwWX45ys2jtwTAusAKJBvzaSZ9aKx0AfxwRPO6HvC6X7HKF7EUw3gtSyvrHPwdtdHc259LZ1cLFeMacvDNaRB9vraPVLni_c7g1lJfsdBdhadWZVg_GC1XGWHnZpMERXDwPRzhAXo7kO5SWbQJJv1f9PMn1vaxyIDpbholxqWxKTp6JBLe_LbirTxZwDHZUkj1lIThfRHUzqypwQbwZL_l0lqUVaOcsgTbhCDfzTWHdEN4a2DE1wKxPWLYOXQgBrNYmR0ZVYVrEh2h_llMSosJ55ASuejIVRjSpZSBcBELL2LplM_qZrKpxFpO0qxKr9N78r5tcQhpcRkd0j1HCqvqqOTNICkEMRcNphQYRCOejQsRXIgyAAk";

            var pubKey = "-----BEGIN PUBLIC KEY-----\r\nMIICIjANBgkqhkiG9w0BAQEFAAOCAg8AMIICCgKCAgEAxPl0/QnZMHXp5k8lx9qr\r\n/dcjDa4PLqfD8+nUEPFO+rO29A6cUcwQgSKK5HFyEylAYqiIOH07d6wMz2FGKKPr\r\nc3xHGvJMQVdGdpl693mQWjqlJMkjmq+z309Miqas356wktOAz7VMLhvJ80ScPHC1\r\nxeEivwrMV/pCjD/o6DinLYevHT/yklXfY79Np9cU103jbh6Ax6q4rkbRiqCgaG9H\r\nFIogmMR4w428imyGXcqsU+PWWIjYgvawel37U6DiDxLTBN3Q5w7tyJpi2+JY43Ib\r\n2GJb8VVKGsUynKA0sCCqiNVLPHwy3yTrHdQJLbb/S65HBWEIXM5PwxWwHMZZPwnZ\r\nLnoBz3HH45aciVV/R14crqTASph2z4yr6aRDqL9OOs+Qh8vDhWsdHAn2AHmMycGh\r\nAxO4T9ohqAqLcTzTgWPizQ5sJRtyvj0ByXfvY6wy31iS38zEeS3ILDA5sPVPkJ6a\r\nK46ojDZNiRHWjaCmotih/coxUlUeFkQ+uXPd8Wj23JtPWeAzjdJfSLunHJDe04FX\r\nhWFAeIA0EAzQJG3guN6qrCuHSYl4j3WHV193ymFzQkKgqkqMOpp1JmqEhMUuzeOq\r\n0dlzZXpktuGtJDSFsxzus6//xJ8QVuEq0Uv3TiROPnBA2nq3FsUzXAwcW75Nqhse\r\nXqaPZlTNOKk7fTj70vNuPucCAwEAAQ==\r\n-----END PUBLIC KEY-----\r\n";
            var rsaPublicKey = RSA.Create();
            rsaPublicKey.ImportFromPem(pubKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuer = true,
                ValidAudience = "test2",
                ValidIssuer = "Application1",
                IssuerSigningKey = new RsaSecurityKey(rsaPublicKey)
            };

            Assert.Throws<SecurityTokenInvalidAudienceException>(() => _systemUnderTest?.DecodeJwt(jwt, validationParameters));
        }

        [Test]
        public void DecodeJwt_Invalid_Token_InvalidSignature()
        {
            var jwt = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJTb21lQ2xhaW0iOiJkYXRhIiwibmJmIjoxNjg5MTY4ODg2LCJleHAiOjE2ODkxNzI0ODYsImlzcyI6IkFwcGxpY2F0aW9uMSIsImF1ZCI6InRlc3QxIn0.DHu5ARcSGrcnuy5Z122CrJqT0McVWBkprp67Rb6iavlLeNxTA0f-e-wF7IZhWnj2hRGrzcRTHp8YzgZqNrup-OC6xC4OyxWahfNvb5ZznPm87F_JBNRD6sMaZA0B3EU0MLwmqMA_RWSt8i_tSM6vZqYPQvLFnd-H6ysqSrhBdxhBaalaJuDxKc-5hiHKPbC0Rk3pGr-PdvFIq9sydLxXgSq_lUkAVS81Vl-JxIP1BTbYtoQiyn8AveaI6JB4bubmVrkDW9OeSEOFYDQ_dSebV6VcrasYrA9uQOEBF6L7iCKAFHNbDnt3LsxwWX45ys2jtwTAusAKJBvzaSZ9aKx0AfxwRPO6HvC6X7HKF7EUw3gtSyvrHPwdtdHc259LZ1cLFeMacvDNaRB9vraPVLni_c7g1lJfsdBdhadWZVg_GC1XGWHnZpMERXDwPRzhAXo7kO5SWbQJJv1f9PMn1vaxyIDpbholxqWxKTp6JBLe_LbirTxZwDHZUkj1lIThfRHUzqypwQbwZL_l0lqUVaOcsgTbhCDfzTWHdEN4a2DE1wKxPWLYOXQgBrNYmR0ZVYVrEh2h_llMSosJ55ASuejIVRjSpZSBcBELL2LplM_qZrKpxFpO0qxKr9N78r5tcQhpcRkd0j1HCqvqqOTNICkEMRcNphQYRCOejQsRXIgyAAk";

            var pubKey = "-----BEGIN PUBLIC KEY-----\r\nMIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCpxiK/t8uwTnch8MLn+eDo+YC3\r\nq4XhCKDaGO9NYp8xyVzm4p9sHYupgtqG6pSIl+zbE5OlgqI02Ff1lTuV3TQJqyPT\r\ny2HXqk4i69xIIw2+Vace9hYG052yqdXs9v9uVFOveZE6lD12gsMSSDL6KhCHGbT9\r\n3rpSAwrFee5mWLCF5wIDAQAB\r\n-----END PUBLIC KEY-----";
            var rsaPublicKey = RSA.Create();
            rsaPublicKey.ImportFromPem(pubKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuer = true,
                ValidAudience = "test1",
                ValidIssuer = "Application1",
                IssuerSigningKey = new RsaSecurityKey(rsaPublicKey)
            };

            Assert.Throws<SecurityTokenSignatureKeyNotFoundException>(() => _systemUnderTest?.DecodeJwt(jwt, validationParameters));
        }
    }
}
