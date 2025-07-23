# ExceptionHub

**ExceptionHub** is a lightweight and modular exception mapping library for .NET 8/9+ applications, providing centralized exception classification for ASP.NET Core, MediatR, MassTransit, and BackgroundServices without enforcing any logging, telemetry, or problem-handling strategy.

---

## 🔧 Features

- ✅ Centralized exception-to-metadata mapping
- ✅ ASP.NET Core global exception handler (IExceptionHandler)
- ✅ MediatR pipeline behavior integration
- ✅ MassTransit consumer pipeline filter
- ✅ BackgroundService safety wrapper via decorator
- ✅ Fluent configuration API with error codes & status codes
- ✅ Custom exception mappers via DI

---

## 🚀 Usage

### 1. Register in Startup

```csharp
builder.Services.AddExceptionHub(options =>
{
    options.Map<MyDomainException>(400, "my_domain_error", ErrorType.Validation);
});
```

### 2. Add to middleware pipeline

```csharp
app.UseExceptionHub();
```

### 3. For Background Services

```csharp
builder.Services.AddExceptionHubHostedService<MyWorker>();
```

### 4. For MediatR

Registered automatically via `IPipelineBehavior`.

### 5. For MassTransit

```csharp
cfg.UseExceptionHub(); // inside your bus registration block
```

---

## 🧩 Extendability

You can implement your own `IExceptionMapper` to match custom exception patterns or domain logic.

```csharp
public class MyDomainMapper : IExceptionMapper
{
    public ExceptionMapping? Map(Exception ex) =>
        ex is MyBusinessException ? new(422, "business_error", ErrorType.Conflict) : null;
}
```

Register with DI:

```csharp
services.AddSingleton<IExceptionMapper, MyDomainMapper>();
```

---

## 🧪 Tested With

- .NET 8 & 9
- ASP.NET Core minimal APIs & MVC
- MediatR 13+
- MassTransit 8.5+
- Hosted services & BackgroundService

---

## ⚖️ License

MIT License © 2025 Berkay Huz
