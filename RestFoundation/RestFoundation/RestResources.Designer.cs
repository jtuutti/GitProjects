﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RestFoundation {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class RestResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal RestResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("RestFoundation.RestResources", typeof(RestResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to REST Foundation has already been configured..
        /// </summary>
        internal static string AlreadyConfigured {
            get {
                return ResourceManager.GetString("AlreadyConfigured", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The relative URL &apos;{0}&apos; has already been mapped..
        /// </summary>
        internal static string AlreadyMapped {
            get {
                return ResourceManager.GetString("AlreadyMapped", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to REST Foundation configuration has not been initialized. Make sure to add a call to one of the Initialize methods of the RestFoundation.Rest.Configuration object on the application start..
        /// </summary>
        internal static string ConfigurationNotInitialized {
            get {
                return ResourceManager.GetString("ConfigurationNotInitialized", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Route parameter &apos;{0}&apos; with value &apos;{1}&apos; does not match the constraint pattern &apos;{2}&apos;..
        /// </summary>
        internal static string ConstraintMismatchedRouteParameter {
            get {
                return ResourceManager.GetString("ConstraintMismatchedRouteParameter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There was a problem registering a service: {0}.
        /// </summary>
        internal static string DependencyRegistrationError {
            get {
                return ResourceManager.GetString("DependencyRegistrationError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There was a problem resolving a service: {0}.
        /// </summary>
        internal static string DependencyResolutionError {
            get {
                return ResourceManager.GetString("DependencyResolutionError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to HTTP method is not allowed..
        /// </summary>
        internal static string DisallowedHttpMethod {
            get {
                return ResourceManager.GetString("DisallowedHttpMethod", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Media type parameters are not allowed..
        /// </summary>
        internal static string DisallowedMediaTypeParameters {
            get {
                return ResourceManager.GetString("DisallowedMediaTypeParameters", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Only a single authentication behavior can be associated with a service method. That includes global behaviors..
        /// </summary>
        internal static string DuplicateAuthenticationBehavior {
            get {
                return ResourceManager.GetString("DuplicateAuthenticationBehavior", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Multiple global service behaviors of the same type are not allowed..
        /// </summary>
        internal static string DuplicateGlobalBehaviors {
            get {
                return ResourceManager.GetString("DuplicateGlobalBehaviors", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Multiple service behaviors of the same type are not allowed for the same route..
        /// </summary>
        internal static string DuplicateRouteBehaviors {
            get {
                return ResourceManager.GetString("DuplicateRouteBehaviors", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to File name cannot be empty..
        /// </summary>
        internal static string EmptyFileName {
            get {
                return ResourceManager.GetString("EmptyFileName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to HTTP headers cannot be empty or have white-space in the name..
        /// </summary>
        internal static string EmptyHttpHeader {
            get {
                return ResourceManager.GetString("EmptyHttpHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to HTTP request failed to process..
        /// </summary>
        internal static string FailedRequest {
            get {
                return ResourceManager.GetString("FailedRequest", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Only a file name can be specified. Relative or absolute paths/URLs are not supported..
        /// </summary>
        internal static string FileNameContainsPath {
            get {
                return ResourceManager.GetString("FileNameContainsPath", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Forbidden.
        /// </summary>
        internal static string Forbidden {
            get {
                return ResourceManager.GetString("Forbidden", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to HTTPS required.
        /// </summary>
        internal static string HttpsRequiredStatusDescription {
            get {
                return ResourceManager.GetString("HttpsRequiredStatusDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The argument contains invalid elements..
        /// </summary>
        internal static string InvalidArgumentValue {
            get {
                return ResourceManager.GetString("InvalidArgumentValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid asynchronous service method timeout provided..
        /// </summary>
        internal static string InvalidAsyncServiceTimeout {
            get {
                return ResourceManager.GetString("InvalidAsyncServiceTimeout", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Connection timeout is invalid..
        /// </summary>
        internal static string InvalidConnectionTimeout {
            get {
                return ResourceManager.GetString("InvalidConnectionTimeout", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No valid file path/URL provided..
        /// </summary>
        internal static string InvalidFilePathOrUrl {
            get {
                return ResourceManager.GetString("InvalidFilePathOrUrl", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid hash key provided..
        /// </summary>
        internal static string InvalidHashKey {
            get {
                return ResourceManager.GetString("InvalidHashKey", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to FromBody type binder requires POST, PUT or PATCH HTTP method..
        /// </summary>
        internal static string InvalidHttpMethodForFromBodyBinder {
            get {
                return ResourceManager.GetString("InvalidHttpMethodForFromBodyBinder", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A resource cannot be set for the current HTTP method..
        /// </summary>
        internal static string InvalidHttpMethodForResource {
            get {
                return ResourceManager.GetString("InvalidHttpMethodForResource", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Index files can only have .html or .htm extensions..
        /// </summary>
        internal static string InvalidIndexFileException {
            get {
                return ResourceManager.GetString("InvalidIndexFileException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Object type must be IEnumerable&lt;IUploadedFile&gt; or ICollection&lt;IUploadedFile&gt; for the media type..
        /// </summary>
        internal static string InvalidIUploadedFileCollectionType {
            get {
                return ResourceManager.GetString("InvalidIUploadedFileCollectionType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid JSONP callback method provided..
        /// </summary>
        internal static string InvalidJsonPCallback {
            get {
                return ResourceManager.GetString("InvalidJsonPCallback", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid key provided..
        /// </summary>
        internal static string InvalidKey {
            get {
                return ResourceManager.GetString("InvalidKey", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid method parameter name provided..
        /// </summary>
        internal static string InvalidMethodParameterName {
            get {
                return ResourceManager.GetString("InvalidMethodParameterName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid OData parameters provided..
        /// </summary>
        internal static string InvalidODataParameters {
            get {
                return ResourceManager.GetString("InvalidODataParameters", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Port must be greater than 0..
        /// </summary>
        internal static string InvalidPortNumber {
            get {
                return ResourceManager.GetString("InvalidPortNumber", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The type &apos;{0}&apos; is not a valid proxy metadata type..
        /// </summary>
        internal static string InvalidProxyMetadataType {
            get {
                return ResourceManager.GetString("InvalidProxyMetadataType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid resource body provided..
        /// </summary>
        internal static string InvalidResourceBody {
            get {
                return ResourceManager.GetString("InvalidResourceBody", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A resource example type must be a class implementing the RestFoundation.ServiceProxy.IResourceExample interface..
        /// </summary>
        internal static string InvalidResourceExampleType {
            get {
                return ResourceManager.GetString("InvalidResourceExampleType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid content result format provided..
        /// </summary>
        internal static string InvalidResultContentFormat {
            get {
                return ResourceManager.GetString("InvalidResultContentFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Service contract type must be an interface or a concrete class that defines its own contract..
        /// </summary>
        internal static string InvalidServiceContract {
            get {
                return ResourceManager.GetString("InvalidServiceContract", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A service implementation that defines its own contract must be non-abstract and marked with the &apos;ServiceContract&apos; attribute..
        /// </summary>
        internal static string InvalidServiceImplementation {
            get {
                return ResourceManager.GetString("InvalidServiceImplementation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No valid service method provided..
        /// </summary>
        internal static string InvalidServiceMethod {
            get {
                return ResourceManager.GetString("InvalidServiceMethod", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There was a problem validating service method argument &apos;{0}&apos;..
        /// </summary>
        internal static string InvalidServiceMethodArgument {
            get {
                return ResourceManager.GetString("InvalidServiceMethodArgument", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid service method lambda expression provided..
        /// </summary>
        internal static string InvalidServiceMethodExpression {
            get {
                return ResourceManager.GetString("InvalidServiceMethodExpression", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid service URL or method lambda expression provided..
        /// </summary>
        internal static string InvalidServiceUrlOrMethodExpression {
            get {
                return ResourceManager.GetString("InvalidServiceUrlOrMethodExpression", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Socket timeout is invalid..
        /// </summary>
        internal static string InvalidSocketTimeout {
            get {
                return ResourceManager.GetString("InvalidSocketTimeout", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The returned task result cannot be started inside the service. Make sure to define tasks returned from services using the new Task&lt;T&gt;() constructor instead of the Task&lt;T&gt;.Factory.StartNew() method..
        /// </summary>
        internal static string InvalidStateOfReturnedTask {
            get {
                return ResourceManager.GetString("InvalidStateOfReturnedTask", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The upper bound address is from a different family than the lower bound address..
        /// </summary>
        internal static string InvalidUpperBoundAddress {
            get {
                return ResourceManager.GetString("InvalidUpperBoundAddress", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Virtual URL must start with ~/.
        /// </summary>
        internal static string InvalidVirtualUrl {
            get {
                return ResourceManager.GetString("InvalidVirtualUrl", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid web response provided..
        /// </summary>
        internal static string InvalidWebResponse {
            get {
                return ResourceManager.GetString("InvalidWebResponse", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to HTTP method OPTIONS cannot be manually defined on a service method..
        /// </summary>
        internal static string ManuallyDefinedOptionsHttpMethod {
            get {
                return ResourceManager.GetString("ManuallyDefinedOptionsHttpMethod", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No matching service type or service method was found..
        /// </summary>
        internal static string MismatchedServiceMethod {
            get {
                return ResourceManager.GetString("MismatchedServiceMethod", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Service method delegate value of the argument &apos;{0}&apos; does not match the corresponding route value: {1} != {2}..
        /// </summary>
        internal static string MismatchedServiceMethodExpression {
            get {
                return ResourceManager.GetString("MismatchedServiceMethodExpression", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Provided service method delegate does not match the route..
        /// </summary>
        internal static string MismatchedServiceMethodRoute {
            get {
                return ResourceManager.GetString("MismatchedServiceMethodRoute", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to URL &apos;{0}&apos; does not match any routes..
        /// </summary>
        internal static string MismatchedUrl {
            get {
                return ResourceManager.GetString("MismatchedUrl", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No authorization manager could be found..
        /// </summary>
        internal static string MissingAuthorizationManager {
            get {
                return ResourceManager.GetString("MissingAuthorizationManager", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No current service method has been set..
        /// </summary>
        internal static string MissingCurrentServiceMethod {
            get {
                return ResourceManager.GetString("MissingCurrentServiceMethod", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Created type does not have a default constructor..
        /// </summary>
        internal static string MissingDefaultConstructor {
            get {
                return ResourceManager.GetString("MissingDefaultConstructor", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No description provided.
        /// </summary>
        internal static string MissingDescription {
            get {
                return ResourceManager.GetString("MissingDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No HTTP context was found..
        /// </summary>
        internal static string MissingHttpContext {
            get {
                return ResourceManager.GetString("MissingHttpContext", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No supported media type was provided in the Accept header..
        /// </summary>
        internal static string MissingOrInvalidAcceptType {
            get {
                return ResourceManager.GetString("MissingOrInvalidAcceptType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No supported media type was provided in the Content-Type header..
        /// </summary>
        internal static string MissingOrInvalidContentType {
            get {
                return ResourceManager.GetString("MissingOrInvalidContentType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Properties must have at least 1 property definition..
        /// </summary>
        internal static string MissingPropertyDefinition {
            get {
                return ResourceManager.GetString("MissingPropertyDefinition", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No REST HTTP module detected. Make sure to add a reference to the RestFoundation.RestHttpModule HTTP module in the Web.config file..
        /// </summary>
        internal static string MissingRestHttpModule {
            get {
                return ResourceManager.GetString("MissingRestHttpModule", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No route data found..
        /// </summary>
        internal static string MissingRouteData {
            get {
                return ResourceManager.GetString("MissingRouteData", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No route handler was passed to the service method invoker..
        /// </summary>
        internal static string MissingRouteHandler {
            get {
                return ResourceManager.GetString("MissingRouteHandler", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No service context was passed to the service method invoker..
        /// </summary>
        internal static string MissingServiceContext {
            get {
                return ResourceManager.GetString("MissingServiceContext", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No supported media types are defined for the media type formatter of type &apos;{0}&apos;..
        /// </summary>
        internal static string MissingSupportedMediaTypeForFormatter {
            get {
                return ResourceManager.GetString("MissingSupportedMediaTypeForFormatter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Parameter &apos;{0}&apos; of the method &apos;{1}&apos; in the service contract of type &apos;{2}&apos; is decorated with multiple type binder attributes..
        /// </summary>
        internal static string MultipleTypeBindersPerParameter {
            get {
                return ResourceManager.GetString("MultipleTypeBindersPerParameter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No accepted charset was provided in the Accept-Charset header..
        /// </summary>
        internal static string NonAcceptedContentCharset {
            get {
                return ResourceManager.GetString("NonAcceptedContentCharset", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The accepted language provided in the Accept-Language header is not supported..
        /// </summary>
        internal static string NonAcceptedContentLanguage {
            get {
                return ResourceManager.GetString("NonAcceptedContentLanguage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Resulting media type is not accepted by the client.
        /// </summary>
        internal static string NonAcceptedMediaType {
            get {
                return ResourceManager.GetString("NonAcceptedMediaType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The output type is not a string..
        /// </summary>
        internal static string NonStringOutputType {
            get {
                return ResourceManager.GetString("NonStringOutputType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Not Found.
        /// </summary>
        internal static string NotFound {
            get {
                return ResourceManager.GetString("NotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An accepted name cannot be null or contain a wildcard..
        /// </summary>
        internal static string NullOrInvalidAcceptedName {
            get {
                return ResourceManager.GetString("NullOrInvalidAcceptedName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Resource body cannot be null..
        /// </summary>
        internal static string NullResourceBody {
            get {
                return ResourceManager.GetString("NullResourceBody", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Asynchronous service method timeout cannot be less than 1 second..
        /// </summary>
        internal static string OutOfRangeAsyncServiceTimeout {
            get {
                return ResourceManager.GetString("OutOfRangeAsyncServiceTimeout", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Method argument &apos;{0}&apos; has a value that is to complex to process. It must be a simple constant or variable value..
        /// </summary>
        internal static string OvercomplicatedMethodArgument {
            get {
                return ResourceManager.GetString("OvercomplicatedMethodArgument", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Service proxy UI is already enabled..
        /// </summary>
        internal static string ProxyAlreadyInitialized {
            get {
                return ResourceManager.GetString("ProxyAlreadyInitialized", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -- SERVICE CALL ENDED --.
        /// </summary>
        internal static string ServiceCallEnded {
            get {
                return ResourceManager.GetString("ServiceCallEnded", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -- SERVICE CALL STARTED --.
        /// </summary>
        internal static string ServiceCallStarted {
            get {
                return ResourceManager.GetString("ServiceCallStarted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Service timed out..
        /// </summary>
        internal static string ServiceTimedOut {
            get {
                return ResourceManager.GetString("ServiceTimedOut", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Service is not available.
        /// </summary>
        internal static string ServiceUnavailable {
            get {
                return ResourceManager.GetString("ServiceUnavailable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Operation is successful.
        /// </summary>
        internal static string SuccessfulOperation {
            get {
                return ResourceManager.GetString("SuccessfulOperation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cache could not be initialized..
        /// </summary>
        internal static string UnableToInitializeCache {
            get {
                return ResourceManager.GetString("UnableToInitializeCache", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Index page file could not be loaded. Make sure the specified index file is available and not locked..
        /// </summary>
        internal static string UnableToLoadIndexPage {
            get {
                return ResourceManager.GetString("UnableToLoadIndexPage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unauthorized.
        /// </summary>
        internal static string Unauthorized {
            get {
                return ResourceManager.GetString("Unauthorized", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to HTTP resource type provided has not been mapped..
        /// </summary>
        internal static string UnmappedResourceType {
            get {
                return ResourceManager.GetString("UnmappedResourceType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No log writer has been registered..
        /// </summary>
        internal static string UnregisteredLogWriter {
            get {
                return ResourceManager.GetString("UnregisteredLogWriter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Range not satisfiable..
        /// </summary>
        internal static string UnsatisfiableRequestedRange {
            get {
                return ResourceManager.GetString("UnsatisfiableRequestedRange", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Service method behavior attribute class &apos;{0}&apos; should be sealed for performance benefits..
        /// </summary>
        internal static string UnsealedBehaviorAttributeClass {
            get {
                return ResourceManager.GetString("UnsealedBehaviorAttributeClass", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The content charset provided is not supported..
        /// </summary>
        internal static string UnsupportedContentCharset {
            get {
                return ResourceManager.GetString("UnsupportedContentCharset", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The content language provided is not supported..
        /// </summary>
        internal static string UnsupportedContentLanguage {
            get {
                return ResourceManager.GetString("UnsupportedContentLanguage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Array parameters are not supported by the FromHeader type binder..
        /// </summary>
        internal static string UnsupportedFromHeaderBinderParameter {
            get {
                return ResourceManager.GetString("UnsupportedFromHeaderBinderParameter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to HTTP Method &apos;{0}&apos; is not supported..
        /// </summary>
        internal static string UnsupportedHttpMethod {
            get {
                return ResourceManager.GetString("UnsupportedHttpMethod", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Media type is not supported.
        /// </summary>
        internal static string UnsupportedMediaType {
            get {
                return ResourceManager.GetString("UnsupportedMediaType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to OData operations are not supported for anonymous objects..
        /// </summary>
        internal static string UnsupportedObjectTypeForOData {
            get {
                return ResourceManager.GetString("UnsupportedObjectTypeForOData", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unsupported operation.
        /// </summary>
        internal static string UnsupportedOperation {
            get {
                return ResourceManager.GetString("UnsupportedOperation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The service is configured not to list its contents in the requested format..
        /// </summary>
        internal static string UnsupportedRequestedFormat {
            get {
                return ResourceManager.GetString("UnsupportedRequestedFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Async route handler does not support synchronous requests..
        /// </summary>
        internal static string UnsupportedSyncRequestForAsyncHandler {
            get {
                return ResourceManager.GetString("UnsupportedSyncRequestForAsyncHandler", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A potentially dangerous value was found in the HTTP request..
        /// </summary>
        internal static string ValidationRequestFailed {
            get {
                return ResourceManager.GetString("ValidationRequestFailed", resourceCulture);
            }
        }
    }
}
