# Copilot Instructions

## General Guidelines
- Prefer using the latest versions of all packages and avoid downgrades whenever possible.
- Prefer clean, readable, maintainable code.
- Avoid duplicated code when possible; prefer reusing existing logic and components.
- Avoid hard-coded styling in Razor pages; prefer using component/page CSS files instead when possible and convenient.

## Responsive Design
- Build the site desktop-first since desktop usage is the primary target.
- Treat mobile as the exception and use media queries for mobile-specific adjustments.
- Use `768px` as the universal breakpoint for mobile devices.

## Code Comments

- Do not add comments to generated code unless the code does something unintuitive or requires explanation.
- Never remove existing comments from the code.

## Data Verification

- Source data used for app verification is located in `wwwroot/sample-data/`.
- Always double-check calculations and displayed values against the files in `wwwroot/sample-data/`, since that is the data the user sees when using the app.
