# IRO.Mvc.PureBinding

Alternative model binder for asp.net mvc. Same to `[FromBody]`, but can take several parameters.

Allow you not to create a model for each method, where you need to two or more parameters.

#### When we work with FromBody:

```csharp
    public class PureBindingTestModel
    {
        public string str { get; set; }
        public int num { get; set; }
    }
```

```csharp
		[HttpPost]
        public JsonResult TestFromBody([FromBody]PureBindingTestModel model)
        {
            return new JsonResult(model);
        }
```

#### When work with FromPureBinding:

```csharp
		[HttpPost]
        public JsonResult TestPureBinding([FromPureBinding]string str, [FromPureBinding]int num)
        {
            return new JsonResult(new object[] { str, num });
        }
```

### Warning!

The fate of the project is unknown, too difficult to maintain.