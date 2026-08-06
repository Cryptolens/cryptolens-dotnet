using System.Net;
using System.Net.Sockets;
using System.Reflection;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SKM.V3.Internal;
using SKM.V3.Models;

namespace Cryptolens.Licensing.Tests;

[TestClass]
[DoNotParallelize]
public class HelperMethodsDiagnosticsTests
{
    private const string RequestTokenSecret = "REQUEST_TOKEN_SECRET";
    private const string ResponseBodySecret = "RESPONSE_BODY_SECRET";

    [TestCleanup]
    public void RestoreGlobalSettings()
    {
        HelperMethods.KeepAlive = true;
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void SuccessfulResponseIsPreserved(bool keepAlive)
    {
        using LoopbackServer server = new LoopbackServer(
            200,
            "OK",
            """{"result":0,"message":null,"products":[]}""");

        GetProductsResult result = GetProducts(server.BaseUrl, keepAlive);

        Assert.IsNotNull(result);
        Assert.AreEqual(ResultType.Success, result.Result);
        Assert.IsNotNull(result.Products);
        Assert.AreEqual(0, result.Products.Count);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void ApiErrorReturnedWithHttp200IsPreserved(bool keepAlive)
    {
        using LoopbackServer server = new LoopbackServer(
            200,
            "OK",
            """{"result":1,"message":"API_ERROR_200"}""");

        GetProductsResult result = GetProducts(server.BaseUrl, keepAlive);

        Assert.AreEqual(ResultType.Error, result.Result);
        Assert.AreEqual("API_ERROR_200", result.Message);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void ApiErrorReturnedWithNonSuccessStatusIsPreserved(bool keepAlive)
    {
        using LoopbackServer server = new LoopbackServer(
            400,
            "Bad Request",
            """{"result":1,"message":"API_ERROR_400"}""");

        GetProductsResult result = GetProducts(server.BaseUrl, keepAlive);

        Assert.AreEqual(ResultType.Error, result.Result);
        Assert.AreEqual("API_ERROR_400", result.Message);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void SuccessStatusWithMalformedJsonReturnsDeserializationError(bool keepAlive)
    {
        using LoopbackServer server = new LoopbackServer(200, "OK", ResponseBodySecret);

        GetProductsResult result = GetProducts(server.BaseUrl, keepAlive);

        AssertDiagnostic(result, "The Cryptolens SDK could not deserialize the server response.");
        StringAssert.Contains(result.Message, "JsonReaderException");
        Assert.IsFalse(result.Message.Contains(RequestTokenSecret, StringComparison.Ordinal));
        Assert.IsFalse(result.Message.Contains(ResponseBodySecret, StringComparison.Ordinal));
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void ErrorStatusWithMalformedJsonReturnsDeserializationError(bool keepAlive)
    {
        using LoopbackServer server = new LoopbackServer(500, "Server Error", ResponseBodySecret);

        GetProductsResult result = GetProducts(server.BaseUrl, keepAlive);

        AssertDiagnostic(result, "The Cryptolens SDK could not deserialize the server response.");
        StringAssert.Contains(result.Message, "JsonReaderException");
        Assert.IsFalse(result.Message.Contains(ResponseBodySecret, StringComparison.Ordinal));
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void JsonNullReturnsDeserializationErrorForSuccessAndErrorStatuses(bool keepAlive)
    {
        using (LoopbackServer successServer = new LoopbackServer(200, "OK", "null"))
        {
            GetProductsResult successResult = GetProducts(successServer.BaseUrl, keepAlive);
            AssertDiagnostic(successResult, "The Cryptolens SDK could not deserialize the server response.");
            StringAssert.Contains(successResult.Message, "JsonSerializationException");
        }

        using (LoopbackServer errorServer = new LoopbackServer(500, "Server Error", "null"))
        {
            GetProductsResult errorResult = GetProducts(errorServer.BaseUrl, keepAlive);
            AssertDiagnostic(errorResult, "The Cryptolens SDK could not deserialize the server response.");
            StringAssert.Contains(errorResult.Message, "JsonSerializationException");
        }
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void ErrorStatusWithoutBodyReturnsTransportError(bool keepAlive)
    {
        using LoopbackServer server = new LoopbackServer(503, "Service Unavailable", string.Empty);

        GetProductsResult result = GetProducts(server.BaseUrl, keepAlive);

        AssertDiagnostic(result, "The Cryptolens SDK could not contact the server.");
        StringAssert.Contains(result.Message, "WebException");
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void DeserializableSuccessBodyOnErrorStatusIsPreserved(bool keepAlive)
    {
        using LoopbackServer server = new LoopbackServer(
            500,
            "Server Error",
            """{"result":0,"message":null,"products":[]}""");

        GetProductsResult result = GetProducts(server.BaseUrl, keepAlive);

        Assert.IsNotNull(result);
        Assert.AreEqual(ResultType.Success, result.Result);
        Assert.IsNotNull(result.Products);
        Assert.AreEqual(0, result.Products.Count);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void UnreachableEndpointReturnsTransportError(bool keepAlive)
    {
        int unusedPort = ReserveUnusedPort();

        GetProductsResult result = GetProducts("http://127.0.0.1:" + unusedPort, keepAlive);

        AssertDiagnostic(result, "The Cryptolens SDK could not contact the server.");
        StringAssert.Contains(result.Message, "WebException");
        Assert.IsFalse(result.Message.Contains(RequestTokenSecret, StringComparison.Ordinal));
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void NullRequestReturnsUnexpectedLocalError(bool keepAlive)
    {
        HelperMethods.KeepAlive = keepAlive;

        GetProductsResult result = SKM.V3.Methods.ProductMethods.GetProducts(RequestTokenSecret, null!);

        AssertDiagnostic(result, "The Cryptolens SDK encountered an unexpected error.");
        StringAssert.Contains(result.Message, "NullReferenceException");
        Assert.IsFalse(result.Message.Contains(RequestTokenSecret, StringComparison.Ordinal));
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void LocalExceptionMessageIsSanitizedAndLengthLimited(bool keepAlive)
    {
        HelperMethods.KeepAlive = keepAlive;
        RequestModel request = new ThrowingRequestModel();

        GetProductsResult result = SKM.V3.Methods.ProductMethods.GetProducts(RequestTokenSecret, request);

        AssertDiagnostic(result, "The Cryptolens SDK encountered an unexpected error.");
        StringAssert.Contains(result.Message, "InvalidOperationException");
        Assert.IsFalse(result.Message.Contains('\r'));
        Assert.IsFalse(result.Message.Contains('\n'));
        Assert.IsFalse(result.Message.Contains('\t'));
        Assert.IsTrue(result.Message.Length <= 512);
        Assert.IsFalse(result.Message.Contains(RequestTokenSecret, StringComparison.Ordinal));
        Assert.IsFalse(result.Message.Contains(ThrowingRequestModel.RequestParameterSecret, StringComparison.Ordinal));
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void WrappedFatalExceptionsAreNotConvertedToResults(bool keepAlive)
    {
        HelperMethods.KeepAlive = keepAlive;
        Exception[] fatalExceptions =
        {
            new OutOfMemoryException("fatal"),
            new StackOverflowException("fatal"),
            new AccessViolationException("fatal")
        };

        foreach (Exception fatalException in fatalExceptions)
        {
            Exception? propagatedException = null;

            try
            {
                SKM.V3.Methods.ProductMethods.GetProducts(
                    RequestTokenSecret,
                    new FatalRequestModel(fatalException));
            }
            catch (Exception ex)
            {
                propagatedException = ex;
            }

            Assert.IsNotNull(propagatedException);
            Assert.AreEqual(fatalException.GetType(), propagatedException.GetBaseException().GetType());
        }
    }

    [TestMethod]
    public void PublicGenericHelperStillSupportsNonBasicResultTypes()
    {
        using LoopbackServer server = new LoopbackServer(200, "OK", """{"value":"plain"}""");

        PlainResult? result = HelperMethods.SendRequestToWebAPI3<PlainResult>(
            new RequestModel { LicenseServerUrl = server.BaseUrl },
            "/plain",
            RequestTokenSecret);

        Assert.IsNotNull(result);
        Assert.AreEqual("plain", result.Value);
    }

    [TestMethod]
    public void PublicGenericHelperReturnsDefaultForMalformedSuccessResponse()
    {
        using LoopbackServer server = new LoopbackServer(200, "OK", ResponseBodySecret);

        PlainResult? result = HelperMethods.SendRequestToWebAPI3<PlainResult>(
            new RequestModel { LicenseServerUrl = server.BaseUrl },
            "/plain",
            RequestTokenSecret);

        Assert.IsNull(result);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void PublicGenericHelperReturnsDefaultForMalformedErrorResponse(bool keepAlive)
    {
        HelperMethods.KeepAlive = keepAlive;
        using LoopbackServer server = new LoopbackServer(500, "Server Error", ResponseBodySecret);

        PlainResult? result = HelperMethods.SendRequestToWebAPI3<PlainResult>(
            new RequestModel { LicenseServerUrl = server.BaseUrl },
            "/plain",
            RequestTokenSecret);

        Assert.IsNull(result);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void PublicGenericHelperReturnsDefaultForTransportFailure(bool keepAlive)
    {
        HelperMethods.KeepAlive = keepAlive;
        int unusedPort = ReserveUnusedPort();

        PlainResult? result = HelperMethods.SendRequestToWebAPI3<PlainResult>(
            new RequestModel { LicenseServerUrl = "http://127.0.0.1:" + unusedPort },
            "/plain",
            RequestTokenSecret);

        Assert.IsNull(result);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void PublicGenericHelperReturnsTypedErrorForBasicResultContract(bool keepAlive)
    {
        HelperMethods.KeepAlive = keepAlive;
        using LoopbackServer server = new LoopbackServer(200, "OK", ResponseBodySecret);

        BasicResult result = HelperMethods.SendRequestToWebAPI3<BasicResult>(
            new RequestModel { LicenseServerUrl = server.BaseUrl },
            "/basic",
            RequestTokenSecret);

        AssertDiagnostic(result, "The Cryptolens SDK could not deserialize the server response.");
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void ListDataObjectsShowAllReturnsTypedErrorWithoutProjectingPayload(bool keepAlive)
    {
        HelperMethods.KeepAlive = keepAlive;
        using LoopbackServer server = new LoopbackServer(200, "OK", ResponseBodySecret);

        ListOfDataObjectsResult result = SKM.V3.Methods.Data.ListDataObjects(
            RequestTokenSecret,
            new ListDataObjectsModel
            {
                ShowAll = true,
                LicenseServerUrl = server.BaseUrl
            });

        AssertDiagnostic(result, "The Cryptolens SDK could not deserialize the server response.");
        Assert.IsNull(result.DataObjects);
    }

    [TestMethod]
    public void EveryConcreteBasicResultTypeHasPublicParameterlessConstructor()
    {
        Type[] resultTypes = typeof(BasicResult).Assembly
            .GetTypes()
            .Where(type =>
                type != typeof(BasicResult) &&
                typeof(BasicResult).IsAssignableFrom(type) &&
                !type.IsAbstract)
            .ToArray();

        Assert.IsTrue(resultTypes.Length > 0);
        foreach (Type resultType in resultTypes)
        {
            Assert.IsNotNull(
                resultType.GetConstructor(Type.EmptyTypes),
                resultType.FullName + " must have a public parameterless constructor.");
        }
    }

    [TestMethod]
    public void PublicGenericHelperSignatureAndDefaultsRemainUnchanged()
    {
        MethodInfo method = typeof(HelperMethods)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(candidate =>
                candidate.Name == "SendRequestToWebAPI3" &&
                candidate.IsGenericMethodDefinition);

        ParameterInfo[] parameters = method.GetParameters();
        Assert.AreEqual(5, parameters.Length);
        Assert.AreEqual(typeof(RequestModel), parameters[0].ParameterType);
        Assert.AreEqual(typeof(string), parameters[1].ParameterType);
        Assert.AreEqual(typeof(string), parameters[2].ParameterType);
        Assert.AreEqual(typeof(int), parameters[3].ParameterType);
        Assert.AreEqual(1, parameters[3].DefaultValue);
        Assert.AreEqual(typeof(int), parameters[4].ParameterType);
        Assert.AreEqual(1, parameters[4].DefaultValue);
        Assert.AreEqual(0, method.GetGenericArguments()[0].GetGenericParameterConstraints().Length);
    }

    private static GetProductsResult GetProducts(string baseUrl, bool keepAlive)
    {
        HelperMethods.KeepAlive = keepAlive;
        return SKM.V3.Methods.ProductMethods.GetProducts(
            RequestTokenSecret,
            new RequestModel { LicenseServerUrl = baseUrl });
    }

    private static void AssertDiagnostic(BasicResult result, string prefix)
    {
        Assert.IsNotNull(result);
        Assert.AreEqual(ResultType.Error, result.Result);
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.Message));
        StringAssert.StartsWith(result.Message, prefix);
        Assert.IsTrue(result.Message.Length <= 512);
    }

    private static int ReserveUnusedPort()
    {
        TcpListener listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        int port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    public sealed class PlainResult
    {
        public string? Value { get; set; }
    }

    private sealed class ThrowingRequestModel : RequestModel
    {
        internal const string RequestParameterSecret = "REQUEST_PARAMETER_SECRET";

        public string Secret { get; set; } = RequestParameterSecret;

        public string Failure
        {
            get
            {
                throw new InvalidOperationException(
                    "first line\r\nsecond line\t" + new string('x', 1000));
            }
        }
    }

    private sealed class FatalRequestModel : RequestModel
    {
        private readonly Exception fatalException;

        internal FatalRequestModel(Exception fatalException)
        {
            this.fatalException = fatalException;
        }

        public string Failure
        {
            get
            {
                throw fatalException;
            }
        }
    }
}
