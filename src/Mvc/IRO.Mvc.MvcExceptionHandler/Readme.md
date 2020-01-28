# IRO.Mvc.MvcExceptionHandler

Flexible library that allow you to easy handle all asp.net exceptions (and HTTP codes too).

All examples source code you can find in test project. Launch it and see how it works.

## Getting started

To easy integrate it you needed to:
- create own base exception class for exceptions that can be shown on client (like i always do) and register or register default exceptions.
- bind them to http codes, if needed.

## What it can do by default?

#### It gives to developer one standart way how to handle exceptions.

All error results have: 
- ErrorKey (by default - name of exception);
- HttpCode of error key (you can set one for all).

By default, in release mode it looks like:

```json
{
  "__IsError": true,
  "ErrorKey": "ClientException",
  "InfoUrl": "https://iro.com/errors/ClientException"
}
```

But you can handle how result will be showed, in response you can return whatever you want, just by using one event.

For each exception you can set its own http code and ErrorKey.

#### Catch binded exceptions (and all their inheritors, if needed) and return json response.

#### In debug mode it will also return debug data and link where you can see exception in default **DeveloperExceptionPage**.

#### Bind http codes.

Exception handler will return error data (like ErrorKey) for http responses without content, if http code was binded to ErrorKey or exception.

Example: if you return BadRequest() in controller - it will be returned with binded ErrorKey and else data.

Current function can be disabled.

#### Handle inner exceptions in AggregateException.

Like here. ClientException is registered and it will be returned.

Current function can be disabled too.

```csharp
            Task.Run(() =>
            {
                Task.Run(() =>
                {
                    throw new ClientException();
                }).Wait();
            }).Wait();
```

#### Easy add your data to response in property "AdditionalData".

# Usage example

First, add services^

```csharp
        services.AddMvcExceptionHandler();
```

One rule - call it after `app.UseDeveloperExceptionPage();` or DeveloperExceptionPage will not work.

```csharp
            app.UseMvcExceptionHandler((s) =>
            {
                //Some configurations.
                s.ErrorDescriptionUrlHandler = new FormattedErrorDescriptionUrlHandler("https://iro.com/errors/{0}");
                s.IsDebug = isDebug;
                s.DefaultHttpCode = 500;
                s.InnerExceptionsResolver = InnerExceptionsResolvers.InspectAggregateException;
                s.CanBindByHttpCode = true;
                s.JsonSerializerSettings.Formatting = Formatting.Indented;
                s.OwnExceptionsHandler += (ex) =>
                {
                    Debug.WriteLine("Exception in MvcExceptionHandler --> " + ex.ToString());
                };

                //Custom response handler (without default json response).
                s.FilterBeforeDTO = async (errorContext) =>
                {
                    //Custom error handling. Return true if MvcExceptionHandler must ignore current error,
                    //because it was handled.
                    return false;
                };

                //Register all exceptions.
                s.Mapping((builder) =>
                {
                    //By http code: with http code will be returned errorKey.
                    builder.Register(
                        httpCode: 500,
                        errorKey: "InternalServerError"
                        );
                    builder.Register(
                        httpCode: 403,
                        errorKey: "Forbidden"
                        );

                    //By exception, custom error key.
                    builder.Register<ArgumentNullException>( 
                        httpCode: 555,
                        errorKey: "CustomErrorKey"
                        );

                    //By exception, default ErrorKey and http code.
                    builder.Register<NullReferenceException>();

                    //Alternative registration method.
                    builder.Register((ErrorInfo) new ErrorInfo()
                    {
                        ErrorKey = "MyError",
                        ExceptionType = typeof(NotImplementedException),
                        HttpCode = 556
                    });

                    //Register all inheritors.
                    //Of course, "lazy registration" when exception first time thrown.
                    builder.RegisterAllAssignable<ClientException>(
                        httpCode: 422,
                        errorKeyPrefix: "ClientEx_"
                        );                    
                });
            });
```



