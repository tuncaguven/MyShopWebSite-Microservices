// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace MyShopWebSite.IdentityServer
{
    public static class Config
    {
        public static IEnumerable<ApiResource> ApiResources => new ApiResource[]
        {
            new ApiResource("ResourceCatalog") {Scopes = { "CatalogFullPermission","CatalogReadPermission"}},
            new ApiResource("ResourceDiscount") {Scopes = { "DiscountFullPermission"}},
            new ApiResource("ResourceOrder") {Scopes = { "OrderFullPermission"}},
            new ApiResource("ResourceBasket") {Scopes = { "BasketFullPermission"}},
            new ApiResource(IdentityServerConstants.LocalApi.ScopeName)
        };

        public static IEnumerable<IdentityResource> IdentityResources => new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Email(),
            new IdentityResources.Profile()
        };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("CatalogFullPermission","Full access to Catalog API"),
                new ApiScope("CatalogReadPermission","Read access to Catalog API"),
                new ApiScope("DiscountFullPermission", "Full access to Discount API"),
                new ApiScope("OrderFullPermission","Full access to Order API"),
                new ApiScope("BasketFullPermission","Full access to Basket API"),
                new ApiScope(IdentityServerConstants.LocalApi.ScopeName)
            };

        public static IEnumerable<Client> Clients => new Client[]
        {
            // Visitor - Client Credentials for API access
            new Client
            {
                ClientId = "MyShopWebSiteVisitorId",
                ClientName = "MyShopWebSite Visitor Client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("myshopwebsitesecret".Sha256()) },
                AllowedScopes =
                {
                    "CatalogReadPermission"
                }
            },

            // Manager - Resource Owner Password for user login
            new Client
            {
                ClientId = "MyShopWebSiteManagerId",
                ClientName = "MyShopWebSite Manager Client",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                ClientSecrets = { new Secret("myshopwebsitesecret".Sha256()) },
                AllowedScopes =
                {
                    "CatalogFullPermission",
                    "CatalogReadPermission",
                    "DiscountFullPermission",
                    "OrderFullPermission",
                    "BasketFullPermission",
                    IdentityServerConstants.LocalApi.ScopeName,
                    IdentityServerConstants.StandardScopes.Email,
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile
                },
                AccessTokenLifetime = 3600
            },

            // Admin - Client Credentials for MVC BFF (backend-to-backend)
            new Client
            {
                ClientId = "MyShopWebSiteAdminId",
                ClientName = "MultiShop Admin Client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("myshopwebsitesecret".Sha256()) },
                AllowedScopes =
                {
                    "CatalogFullPermission",
                    "CatalogReadPermission",
                    "DiscountFullPermission",
                    "OrderFullPermission",
                    "BasketFullPermission",
                    IdentityServerConstants.LocalApi.ScopeName
                },
                AccessTokenLifetime = 3600
            },

            // MVC Client - Authorization Code + PKCE for interactive user login
            new Client
            {
                ClientId = "MultiShopMvcClient",
                ClientName = "MultiShop MVC Web Application",
                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true,
                RequireClientSecret = true,
                ClientSecrets = { new Secret("multishopmvcsecret".Sha256()) },
                
                RedirectUris = { "https://localhost:5002/signin-oidc" },
                PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },
                FrontChannelLogoutUri = "https://localhost:5002/signout-oidc",
                
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "CatalogFullPermission",
                    "CatalogReadPermission",
                    "DiscountFullPermission",
                    "OrderFullPermission",
                    "BasketFullPermission"
                },
                
                AllowOfflineAccess = true,
                AccessTokenLifetime = 3600,
                RefreshTokenUsage = TokenUsage.ReUse,
                RefreshTokenExpiration = TokenExpiration.Sliding,
                SlidingRefreshTokenLifetime = 86400
            }
        };
    }
}