﻿using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class AssemblyReference
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}
