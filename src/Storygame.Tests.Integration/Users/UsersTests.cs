using Microsoft.Extensions.DependencyInjection;
using Storygame.Client;
using Storygame.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Storygame.Tests.Integration.Users;

[TestFixture]
public class UsersTests
{
    private static StorygameClient CreateClient()
        => WebAppFactory.CreateStorygameClient(services =>
            services.AddSingleton<IUsersRepository>(new InMemoryUsersRepository()));
}
