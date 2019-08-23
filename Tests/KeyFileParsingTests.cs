using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PrivateKeyParsers;

namespace Tests
{
    /// <summary>
    /// Defines the <see cref="KeyFileParsingTests" />
    /// </summary>
    [TestClass]
    public class KeyFileParsingTests
    {
        [TestMethod]
        public void TestPemPrivateKeyParse()
        {
            string privateKey = @"-----BEGIN RSA PRIVATE KEY-----
MIIBOQIBAAJAYgaexiHMaFzw6PTYd/hf5ZxD6uzrAPO0ixNE2a19/p0vJcI8SfEb
NAjmX330Ely2JR4VePVi8+BsqkHZn5+M3QIDAQABAkAK9xidqzw1VGgydukcCnGX
urIyPkxPb/N7Ny2Vd/3HFGSqVtm42le3Uwj/h8XNJgiqWPmUvPFZ4SM6Yrxa0xVB
AiEAp1E5N7VNdQjkPVqk3d98psECMeOsixCmk1g2lDrwtU0CIQCV+3QHWYRgKkdl
AnJOEuTitvy1LyjacWdqgBrjs7Et0QIgfXKftM4BqjslbX7118jFm/1gWOl8J7Qv
QJBGM7NRYI0CIAXTmPRYI+5gdhmUeMBTt5SfKz4WsO2bjjry8xh5eJ6hAiEAmqtK
8Hm9Glgt9C62n24xTSTUqkmYW2ecmxgK+9jr7UM=
-----END RSA PRIVATE KEY-----";

            string expectedResult = "<RSAKeyValue><Modulus>YgaexiHMaFzw6PTYd/hf5ZxD6uzrAPO0ixNE2a19/p0vJcI8SfEbNAjmX330Ely2JR4VePVi8+BsqkHZn5+M3Q==</Modulus><Exponent>AQAB</Exponent><P>p1E5N7VNdQjkPVqk3d98psECMeOsixCmk1g2lDrwtU0=</P><Q>lft0B1mEYCpHZQJyThLk4rb8tS8o2nFnaoAa47OxLdE=</Q><DP>fXKftM4BqjslbX7118jFm/1gWOl8J7QvQJBGM7NRYI0=</DP><DQ>BdOY9Fgj7mB2GZR4wFO3lJ8rPhaw7ZuOOvLzGHl4nqE=</DQ><InverseQ>mqtK8Hm9Glgt9C62n24xTSTUqkmYW2ecmxgK+9jr7UM=</InverseQ><D>CvcYnas8NVRoMnbpHApxl7qyMj5MT2/zezctlXf9xxRkqlbZuNpXt1MI/4fFzSYIqlj5lLzxWeEjOmK8WtMVQQ==</D></RSAKeyValue>";

            KeyFileParser parser = new KeyFileParser(Encoding.ASCII.GetBytes(privateKey));
            var parameters = parser.GetParameters();

            using (RSACryptoServiceProvider provider = new RSACryptoServiceProvider())
            {
                provider.ImportParameters(parameters);

                Assert.AreEqual(expectedResult, provider.ToXmlString(true));
            }
        }

        [TestMethod]
        public void TestPemPrivateKeyAlternateParse()
        {
            string privateKey = @"-----BEGIN PRIVATE KEY-----
MIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQCdzdZbVcgVB+BX
BaaJLveXoaQZn2d6GSai3GZ4M82nziztbtUSWWXajnKip5xj5/Up4/F3xL8dd5HA
I5TSMIA0JN76BfamgahxxcHZvcLs6+MlNvJD8yO9R0Vp2H4gbGwX4qDaxpnyqJ5L
Mt8jnmDsNhqduge39giuNALLAaS91H+rKJFJWzEtRUi2dJ31glpbM6uFHsFfRk29
IUDT6lqQWrazNHblpvMDE40vf3FMfof5vKqB5w7KF9Nyf/MPVR+Bzv5vC+BwpcNx
4yYQ+3LK3rhvWiaAOwNoj2/ojj3GiU0Lt9H/sMfVAQ56Ru4iUJ4wipN0KzGznIbj
xmu9YsFtAgMBAAECggEAINePhre+Kf5XJtVSuRdQyTqHZBvEVel5HIkBPIAPi56B
xDJ+XtDDPW7LFeYLStGiOQMrJJGKcvAJIfNlzC7y56dKzr9B+5pde7w8IXx4XoWj
YwHh/tlR9VkpbaN0cHwQLRJqBs4xiQql0yG1xfx20IJcHLp0U8l9OgAyGpzvc+A7
kFg+o8mxR4mrQlJSCrxsMgQimyJwXOzCIIv7GkFhpw4ugvIUMbnEqDUAdggqY0xr
+2VNO/dvUNlD5OaHMDi6AlfSq4xsJDAYWelYfIE+md272T0b4UZ4yVsaYep2689d
HJGnS9vBvxg4U9s+nraqcIJ+ntdmZdvAOAfzZ/AxwwKBgQDWnZyFZnT8oLxV59pj
oZRZUmxkdydg0BCRwUcDyuelucmxPlrzyxmn2x+DEU22WAYfZv3ZT2EP+582FSCD
6Vxnuh1MGeGfWAJ3wRiyJ1Gl1zwetEXHqxL+X9wXHx0lYudBHwB54cZX0EMueGZX
sJM4SoCh+yQFGvSj4FkSNi8QlwKBgQC8O8AmRmM3DODilz3jCDdV++fzvkNJ3GBd
/gJHgP3nO/DpNOlLU2w5irQXH85XKSiWhfCIqzlKWxsaWDGqDNnPdLNgThst3+xA
BoiMG9tPr2myvbLakc52Tuh8oD8lA/140HuMdDvFaTQX1/4tbrQJ775gcplok8Xo
B1/1jpq6mwKBgEa8wldR0uNlk035UokeO8hJG4LtpyQI0D1KaD6+xSVhnDH4bIAI
hFdIKRXJQUUFtFbrWmYi/MoI1Iw94G66HwPtWzS3Hx+nIYEZOyuVPBseWyl/n7RV
FbiUHfXdAn1NIQ3cywphOT++XDZX5tumTo/yNn2tSk0IenP9QT54b4DXAoGAGrPb
OVLI5llUWbCc3eUffHok2IYII6U4onzTM/OPkUsGjP6tjbsC4lLT42fmrKSxFlFf
4vNvSCYOfTk/qmDyUSS8AZBy+JoIeLi0jDOzc+VteCbDBZCjmlLtViihbI3ZWlcw
/6bJh+K5uhww+Z73uWUiO6pmKtizvYu2SC53srsCgYARDEZVacFdkKIJEbx1GhFw
z9r+BMlXPs7cfgyoWIFlhEfhMAupqzZNHrB5tkoNEm1Nau2FwLL9PCW3WUHlU2Fd
yVDdsp0RsDtNBko+rD4ZVGS8vsxgJywm68jdKWsMLe+D0/nY7ZOF/Uiyyd3msEem
Q6mjt2bZxmYdHdLescJlRQ==
-----END PRIVATE KEY-----";

            string expectedResult = "<RSAKeyValue><Modulus>nc3WW1XIFQfgVwWmiS73l6GkGZ9nehkmotxmeDPNp84s7W7VElll2o5yoqecY+f1KePxd8S/HXeRwCOU0jCANCTe+gX2poGoccXB2b3C7OvjJTbyQ/MjvUdFadh+IGxsF+Kg2saZ8qieSzLfI55g7DYanboHt/YIrjQCywGkvdR/qyiRSVsxLUVItnSd9YJaWzOrhR7BX0ZNvSFA0+pakFq2szR25abzAxONL39xTH6H+byqgecOyhfTcn/zD1Ufgc7+bwvgcKXDceMmEPtyyt64b1omgDsDaI9v6I49xolNC7fR/7DH1QEOekbuIlCeMIqTdCsxs5yG48ZrvWLBbQ==</Modulus><Exponent>AQAB</Exponent><P>1p2chWZ0/KC8VefaY6GUWVJsZHcnYNAQkcFHA8rnpbnJsT5a88sZp9sfgxFNtlgGH2b92U9hD/ufNhUgg+lcZ7odTBnhn1gCd8EYsidRpdc8HrRFx6sS/l/cFx8dJWLnQR8AeeHGV9BDLnhmV7CTOEqAofskBRr0o+BZEjYvEJc=</P><Q>vDvAJkZjNwzg4pc94wg3Vfvn875DSdxgXf4CR4D95zvw6TTpS1NsOYq0Fx/OVykoloXwiKs5SlsbGlgxqgzZz3SzYE4bLd/sQAaIjBvbT69psr2y2pHOdk7ofKA/JQP9eNB7jHQ7xWk0F9f+LW60Ce++YHKZaJPF6Adf9Y6aups=</Q><DP>RrzCV1HS42WTTflSiR47yEkbgu2nJAjQPUpoPr7FJWGcMfhsgAiEV0gpFclBRQW0VutaZiL8ygjUjD3gbrofA+1bNLcfH6chgRk7K5U8Gx5bKX+ftFUVuJQd9d0CfU0hDdzLCmE5P75cNlfm26ZOj/I2fa1KTQh6c/1BPnhvgNc=</DP><DQ>GrPbOVLI5llUWbCc3eUffHok2IYII6U4onzTM/OPkUsGjP6tjbsC4lLT42fmrKSxFlFf4vNvSCYOfTk/qmDyUSS8AZBy+JoIeLi0jDOzc+VteCbDBZCjmlLtViihbI3ZWlcw/6bJh+K5uhww+Z73uWUiO6pmKtizvYu2SC53srs=</DQ><InverseQ>EQxGVWnBXZCiCRG8dRoRcM/a/gTJVz7O3H4MqFiBZYRH4TALqas2TR6webZKDRJtTWrthcCy/Twlt1lB5VNhXclQ3bKdEbA7TQZKPqw+GVRkvL7MYCcsJuvI3SlrDC3vg9P52O2Thf1Issnd5rBHpkOpo7dm2cZmHR3S3rHCZUU=</InverseQ><D>INePhre+Kf5XJtVSuRdQyTqHZBvEVel5HIkBPIAPi56BxDJ+XtDDPW7LFeYLStGiOQMrJJGKcvAJIfNlzC7y56dKzr9B+5pde7w8IXx4XoWjYwHh/tlR9VkpbaN0cHwQLRJqBs4xiQql0yG1xfx20IJcHLp0U8l9OgAyGpzvc+A7kFg+o8mxR4mrQlJSCrxsMgQimyJwXOzCIIv7GkFhpw4ugvIUMbnEqDUAdggqY0xr+2VNO/dvUNlD5OaHMDi6AlfSq4xsJDAYWelYfIE+md272T0b4UZ4yVsaYep2689dHJGnS9vBvxg4U9s+nraqcIJ+ntdmZdvAOAfzZ/Axww==</D></RSAKeyValue>";

            KeyFileParser parser = new KeyFileParser(Encoding.ASCII.GetBytes(privateKey));
            var parameters = parser.GetParameters();

            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            provider.ImportParameters(parameters);

            Assert.AreEqual(expectedResult, provider.ToXmlString(true));
        }
    }
}