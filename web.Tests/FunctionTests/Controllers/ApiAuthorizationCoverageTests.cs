using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Atlas_Web.Controllers.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Xunit;

namespace web.Tests.FunctionTests.Controllers;

public class ApiAuthorizationCoverageTests
{
    [Fact]
    public void NonAuthApiActions_RequireBearerAuthentication()
    {
        var failures = GetApiControllerTypes()
            .Where(type => type != typeof(AuthApiController))
            .SelectMany(GetHttpActions)
            .Where(action => !HasBearerAuthorization(action.Method, action.Controller))
            .Select(action => $"{action.Controller.Name}.{action.Method.Name}")
            .OrderBy(x => x)
            .ToArray();

        Assert.True(
            failures.Length == 0,
            "Missing bearer authorization: " + string.Join(", ", failures)
        );
    }

    [Fact]
    public void AuthApiActions_OnlyExposeLoginAndLogoutAnonymously()
    {
        var actions = GetHttpActions(typeof(AuthApiController)).ToDictionary(x => x.Method.Name);

        Assert.True(HasAllowAnonymous(actions["Login"].Method, typeof(AuthApiController)));
        Assert.True(HasAllowAnonymous(actions["Logout"].Method, typeof(AuthApiController)));
        Assert.False(HasAllowAnonymous(actions["Me"].Method, typeof(AuthApiController)));
        Assert.True(HasBearerAuthorization(actions["Me"].Method, typeof(AuthApiController)));
    }

    private static IEnumerable<Type> GetApiControllerTypes()
    {
        return typeof(AuthApiController).Assembly
            .GetTypes()
            .Where(type =>
                type.IsClass
                && !type.IsAbstract
                && typeof(ControllerBase).IsAssignableFrom(type)
                && type.Namespace == typeof(AuthApiController).Namespace
                && type.GetCustomAttribute<ApiControllerAttribute>() != null
            );
    }

    private static IEnumerable<(Type Controller, MethodInfo Method)> GetHttpActions(Type controller)
    {
        return controller.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .Where(method => method.GetCustomAttributes().OfType<HttpMethodAttribute>().Any())
            .Select(method => (controller, method));
    }

    private static bool HasAllowAnonymous(MethodInfo method, Type controller)
    {
        return method.GetCustomAttribute<AllowAnonymousAttribute>() != null
            || controller.GetCustomAttribute<AllowAnonymousAttribute>() != null;
    }

    private static bool HasBearerAuthorization(MethodInfo method, Type controller)
    {
        if (HasAllowAnonymous(method, controller))
        {
            return false;
        }

        return GetAuthorizeAttributes(method, controller)
            .Any(attribute => string.Equals(attribute.AuthenticationSchemes, "Bearer", StringComparison.Ordinal));
    }

    private static IEnumerable<AuthorizeAttribute> GetAuthorizeAttributes(MethodInfo method, Type controller)
    {
        return method.GetCustomAttributes<AuthorizeAttribute>()
            .Concat(controller.GetCustomAttributes<AuthorizeAttribute>());
    }
}
