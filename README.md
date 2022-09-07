
# Kentico Xperience Content Reference Module UI (i.e., Content Usage Tab)
Do your Xperience authors need help keeping track of where their reusable content items are used? This module adds a Usage tab to the Kentico page editor, listing all the pages on which a piece of content is reused, through Xperience page relationships, field references, or widget property references.

![Usage tab example](/images/usage-tab.png)

## Purpose
Creating reusable, atomic content, and composing pages using content relationships is becoming increasingly popular with the advent of headless CMS and hybrid CMS platforms. In Kentico Xperience, this typically entails connecting content through page relationships, field references, and widget properties. This module used the [Kentico Xperience Content Reference Module](https://github.com/heywills/xperience-content-reference-module) (a dependency) to index content relationships so that it can provide a **Usage** tab in the Xperience page editor. This tab lists all the pages where a piece of content is reused through page relationships, field references or widget properties.
## Install
1. Add the NuGet package, [KenticoCommunity.ContentReferenceUi](https://github.com/heywills/xperience-content-reference-module-ui) to your Kentico CMS project.
2. Add the package version of [Kentico.Xperience.Libraries](https://www.nuget.org/packages/Kentico.Xperience.Libraries/) that matches your Xperience hotfix version. This is critical to prevent the  13.0.0 version of the Xperience libraries from being used when compiling your project.
3. Build your CMS project.
4. The first time you start the application the [Kentico Xperience Content Reference Module](https://github.com/heywills/xperience-content-reference-module) will create a local Smart Search index named "Content Reference Module Index".

## Uninstall
1. Remove the package KenticoCommunity.ContentReferenceUi from your CMS project.
2. Rebuild the CMS project.
3. Kentico will remove the Module (Resource) and UI Element objects the next time the CMS application starts.

## Troubleshooting
This solution uses Xperience smart search. Ensure the index "Content Reference Module Index" is created, built, and propagated to all members of the web farm.
## Compatibility
* .NET 4.8 or higher for the admin app or MVC5 projects
* Kentico Xperience versions 13.0.0 or higher

## License
This project uses a standard MIT license which can be found here.

## Contribution
Contributions are welcome. Feel free to submit pull requests to the repo.

## Support
Please report bugs as issues in this GitHub repo. We'll respond as soon as possible.

There are existing know issues and plans for this repo here:

https://github.com/heywills/xperience-content-reference-module-ui/issues

