# IRO.Mvc.CoolSwagger

CoolSwagger - some extensions for Swashbuckle (tool to generate swagger.json from controllers).

What it add:
1. Automatically searching for summary files.
1. Own summary generator, that add more info (include "parameter" and "return" summary).
1. Integrated with Identity. 
1. Can set tag name for controller or method with attribute.
1. SwaggerDocAdditional - set additional data.

```csharp
            services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDocAdditional(doc =>
                {
                    doc.Schemes = new List<string>
                    {
                        "http",
                        "https"
                    };
                });

                opt.UseCoolSummaryGen();
                opt.UseDefaultIdentityAuthScheme();
                opt.AddSwaggerTagNameOperationFilter();

                opt.AddPureBindingToSwashbuckle();
            });
```