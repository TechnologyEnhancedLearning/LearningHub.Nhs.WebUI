using AutoFixture;
using AutoFixture.Kernel;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningHub.Nhs.WebUI.BlazorComponents.UnitTests.DI
{
    public class FallbackServiceProvider : IServiceProvider
    {
        private readonly Fixture _fixture;

        public FallbackServiceProvider()
        {
            // Initialize AutoFixture and configure it with Moq
            _fixture = new Fixture();
            _fixture.Customize(new AutoFixture.AutoMoq.AutoMoqCustomization { ConfigureMembers = true });
        }

        public object? GetService(Type serviceType)
        {
            try
            {

                // Prevent AutoFixture from mocking IComponentActivator, which can cause issues
                if (serviceType == typeof(IComponentActivator))
                {
                    return null; // Let Blazor's default DI handle this
                }

                //if (serviceType.IsInterface)
                //{
                //  https://bunit.dev/docs/providing-input/inject-services-into-components.html#using-libraries-like-automocker-as-fallback-provider
                //    return _fixture.Create(serviceType);
                //}

                // If it's not an interface, try creating the real object
                return _fixture.Create(serviceType, new SpecimenContext(this._fixture));
            }
            catch
            {
                // Return null if AutoFixture can't create the service
                return null;
            }
        }
    }
}
