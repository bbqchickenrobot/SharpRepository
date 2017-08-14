﻿using Ninject;
using SharpRepository.Repository;
using SharpRepository.Repository.Configuration;
using System.Reflection;

namespace SharpRepository.Ioc.Ninject
{
    public static class NinjectRepositoryExtensions
    {
        public static void BindSharpRepository(this IKernelConfiguration kernel, string repositoryName = null)
        {
            kernel.Bind(typeof (IRepository<>)).ToMethod(context =>
                {
                    var genericArgs = context.Request.Service.GetTypeInfo().GetGenericArguments();

                    return RepositoryFactory.GetInstance(genericArgs[0], repositoryName);
                });

            kernel.Bind(typeof(IRepository<,>)).ToMethod(context =>
                {
                    var genericArgs = context.Request.Service.GetTypeInfo().GetGenericArguments();

                    return RepositoryFactory.GetInstance(genericArgs[0], genericArgs[1], repositoryName);
                });

            kernel.Bind(typeof(ICompoundKeyRepository<,,>)).ToMethod(context =>
                {
                    var genericArgs = context.Request.Service.GetGenericArguments();

                    return RepositoryFactory.GetInstance(genericArgs[0], genericArgs[1], genericArgs[2], repositoryName);
                });
        }

        public static void BindSharpRepository(this IKernelConfiguration kernel, ISharpRepositoryConfiguration configuration)
        {
            kernel.Bind(typeof (IRepository<>)).ToMethod(context =>
                {
                    var genericArgs = context.Request.Service.GetTypeInfo().GetGenericArguments();

                    return RepositoryFactory.GetInstance(genericArgs[0], configuration);
                });

            kernel.Bind(typeof(IRepository<,>)).ToMethod(context =>
                {
                    var genericArgs = context.Request.Service.GetTypeInfo().GetGenericArguments();

                    return RepositoryFactory.GetInstance(genericArgs[0], genericArgs[1], configuration);
                });

            kernel.Bind(typeof(ICompoundKeyRepository<,,>)).ToMethod(context =>
            {
                var genericArgs = context.Request.Service.GetTypeInfo().GetGenericArguments();

                return RepositoryFactory.GetInstance(genericArgs[0], genericArgs[1], genericArgs[2], configuration);
            });
        }
    }
}
