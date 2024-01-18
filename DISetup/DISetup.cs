using GerenciadorCertificados.Interfaces;
using GerenciadorCertificados.Repositorio;
using SimpleInjector;

namespace GerenciadorCertificados.DISetup
{
    public static class DISetup
    {
        private static readonly Container _container;

        static DISetup()
        {
            _container = new Container();
            _container.RegisterSingleton<ITCertificadoRepositorio, TCertificadoRepositorio>();
            _container.Options.ResolveUnregisteredConcreteTypes = true;
            _container.Verify();
        }

        public static Container Container => _container;
    }
}

