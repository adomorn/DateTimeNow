# DateTimeNow

A Visual Studio extension that replaces `DateTime.Now` or `DateTime.UtcNow` in C# code with an explicit `new DateTime(...)` expression using the time when the code fix is invoked.

This is useful when inserting a fixed timestamp into sample code or a test fixture. Applying the fix changes a runtime clock read into a constant timestamp.

## Install

Install from the [Visual Studio Marketplace](https://marketplace.visualstudio.com/items?itemName=Adomorn.ext1453).

The checked-in VSIX manifest declares Visual Studio 15.x–17.x targets. Compatibility with later versions has not been established in this repository.

## Usage

1. Open a C# file in Visual Studio.
2. Place the cursor on `DateTime.Now` or `DateTime.UtcNow`.
3. Open Quick Actions and apply the DateTimeNow code fix.

The UTC variant includes `DateTimeKind.Utc` in the generated expression.

## Development

Open `DateTimeNow.sln` in a compatible Visual Studio installation on Windows with extension development tooling installed.

- `DateTimeNow/DateTimeNow`: Roslyn analyzer.
- `DateTimeNow/DateTimeNow.CodeFixes`: code fix implementation.
- `DateTimeNow/DateTimeNow.Test`: analyzer test project.
- `DateTimeNow/DateTimeNow.Vsix`: extension packaging and compatibility manifest.

Build and test in Visual Studio before proposing compatibility changes. There is currently no GitHub Actions workflow in this repository.

## License

[MIT](LICENSE).
