using Microsoft.AspNetCore.Mvc.Filters;

namespace PIA_Equipo_11.Filtros
{
    public class FiltroExcepcion: ExceptionFilterAttribute
    {
        private readonly ILogger<FiltroExcepcion> log;
        
        public FiltroExcepcion(ILogger<FiltroExcepcion> log)
        {
            this.log = log;
        }

        public override void OnException(ExceptionContext context)
        {
            log.LogError(context.Exception, context.Exception.Message);
            base.OnException(context);
        }
    }
}
