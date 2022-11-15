# FranceConnect Starter Kit - FranceConnect Facade

**This project is a Work In Progress (WIP) effort**

## About FranceConnect

The [FranceConnect](https://franceconnect.gouv.fr) platform (FCP) is an identification system designed to facilitate user access to the digital online services of the e-government, a.k.a. FranceConnected services, avoiding every French citizen to have to create a new account when accessing a new online service and therefore to remember different passwords for all services accessed.

To do this, FCP allows each user to have an identification mechanism recognized by the administration's online services through the FranceConnect button. When accessing a new service, in addition to the possibility of registering with an administrative authority that the user does not yet know, the button allows the user to select a compatible supported identity that he or she already has, and use it in this context. In terms of supported identity providers (IdP), this platform makes it possible for citizens to use an existing account @ an administrations or an e-gov agency’s website, e.g., IRS and social security. Interestingly, a demo environment is available [here](https://fournisseur-de-service.dev-franceconnect.fr).

FranceConnect is supported by the Interministerial Digital Directorate, or DINUM, which assists ministries in their digital transformation, advises the government and develops shared services and resources, such as the State's online identification and authentication system, or the State's interministerial network, data.gouv.fr or api.gouv.fr.

FranceConnect figures:
- 40+ million users.
- 18 million monthly connections.
- Access to over 1400 online services. Such services are referred as to FranceConnected services.

The FranceConnect service implementation documentation is available on the [partner portal](https://partenaires.franceconnect.gouv.fr/) provided by DINUM.

## About the FranceConnect Facade (FCP)

From a FranceConnected services (a.k.a., service providers (SP)) standpoint, [Dynamics 365](https://dynamics.microsoft.com/) Biz Apps portals, the [Power Pages](https://powerpages.microsoft.com/) websites, formely Power Apps portals, as well as [Azure AD B2C](https://azure.microsoft.com/en-us/services/active-directory/external-identities/b2c/#overview), cannot unfortunately directly integrate with the FranceConnect platform (FCP), while both these offerings and FCP are based on the same industry standard protocol, namely the [OpenID Connect (OIDC) protocol](https://openid.net/specs/openid-connect-core-1_0.html) w/ the authorization code flow. As always, the devil resides in detail. 

in this context, this project both discusses and illustrates a suggested solution via a so-called FranceConnect Facade (FCF), i.e., a lightweight adaptation layer to handle all the identified discrepancies and cope with the related issues, and to ultimately interoperate with FCP from a "plumbing" perspective. 

## Content

This project currently provides the following content:
* [A series of technical-functional specifications (Draft)](https://github.com/microsoft/franceconnect-facade-dotnet-webapp-aspnetcore/tree/master/Specifications) to build such a facade.
* [A code sample in .NET 6 (LTS)](https://github.com/microsoft/franceconnect-facade-dotnet-webapp-aspnetcore/tree/master/Source) to illustrate how to implement such a defined facade. 
- [A "Getting Started" guide](https://github.com/microsoft/franceconnect-facade-dotnet-webapp-aspnetcore/tree/master/Documentation) to help deploy the code sample, and how to test it.
- [A series of Bicep scripts](ttps://github.com/microsoft/franceconnect-facade-dotnet-webapp-aspnetcore/tree/main/Scripts) to help deploy the required resources in Azure, and fulfill the prerequisites that pertains to this deployment.

# About the FranceConnect Facade

** This project is a Work In Progress (WIP) effort **

In 2019, the French government created DINUM (Inter-Ministerial Digital Directorate) to support the digital transformation of the state and all communities. DINUM is an entity very influential attached to the Prime Minister.
DINUM supports ministries, towns, departments and regions. But it is also developing shared services and resources such as FranceConnect.

FranceConnect (FC) is a solution offered by DINUM which makes it possible to secure and simplify the connection of all French citizen with e-administration, and the related online services. See https://franceconnect.gouv.fr, https://partenaires.franceconnect.gouv.fr/. Interestingly, a demo environment is available [here](https://fournisseur-de-service.dev-franceconnect.fr).

FranceConnect figures:
- 40+ million users.
- 18 million monthly connections.
- Access to over 1400 online services. Such services are referred as to FranceConnected services.

In terms of supported identity providers (IdP), this platform makes it possible for citizens to use an existing account @ an administrations or an e-gov agency’s website, e.g., IRS and social security. 

From FranceConnected services (a.k.a., service providers (SP)) standpoint, Dynamics 365 Biz Apps portals, the Power Apps Portals/Power Pages websites, as well as Azure AD B2C, cannot unfortunately directly integrate with the FranceConnect platform (FCP), while both these offerings and FCP are based on the same industry standard protocol, namely the [OpenID Connect (OIDC) protocol](https://openid.net/specs/openid-connect-core-1_0.html) w/ the authorization code flow. As always, the devil resides in detail. 

in this context, this project both discusses and illustrates a suggested solution via a so-called FranceConnect Facade (FCF), i.e., a lightweight adaptation layer to handle all the identified discrepancies and cope with the related issues, and to ultimately interoperate with FCP from a "plumbing" perspective. 

This project currently provides the following content:
* [A series of technical-functional specifications (Draft) to build such a facade](https://github.com/microsoft/franceconnect-facade-dotnet-webapp-aspnetcore/tree/master/specs).
* [A code sample in .NET 6 to illustrate how to implement such a defined facade](https://github.com/microsoft/franceconnect-facade-dotnet-webapp-aspnetcore/tree/master/code). 
- [A "Getting Started" guide to help deploy the illustration code sample first on a local development environment and then on Azure, and how to test it](https://github.com/microsoft/franceconnect-facade-dotnet-webapp-aspnetcore/tree/master/docs).
- [A series of Bicep scripts to help deploy the required resources in Azure, and fulfill the prerequisites that pertains to this deployment](ttps://github.com/microsoft/franceconnect-facade-dotnet-webapp-aspnetcore/tree/main/bicep).

# Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

# Legal Notices

Microsoft and any contributors grant you a license to the Microsoft documentation and other content
in this repository under the [Creative Commons Attribution 4.0 International Public License](https://creativecommons.org/licenses/by/4.0/legalcode),
see the [LICENSE](LICENSE) file, and grant you a license to any code in the repository under the [MIT License](https://opensource.org/licenses/MIT), see the
[LICENSE-CODE](LICENSE-CODE) file.

Microsoft, Windows, Microsoft Azure and/or other Microsoft products and services referenced in the documentation
may be either trademarks or registered trademarks of Microsoft in the United States and/or other countries.
The licenses for this project do not grant you rights to use any Microsoft names, logos, or trademarks.
Microsoft's general trademark guidelines can be found at http://go.microsoft.com/fwlink/?LinkID=254653.

Privacy information can be found at https://privacy.microsoft.com/en-us/

Microsoft and any contributors reserve all other rights, whether under their respective copyrights, patents,
or trademarks, whether by implication, estoppel or otherwise.
