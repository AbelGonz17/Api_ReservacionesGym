namespace ApiReservacionesGym.Middleawares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ErrorHandlingMiddleware> logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context); // Sigue al siguiente middleware o controller
            }
            catch (Exception ex)
            {
                logger.LogError(ex, " Error no manejado: {Mensaje}", ex.Message);

                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                var respuesta = new
                {
                    mensaje = "Ocurrio un error inesperado.",
                    detalle = ex.Message
                };

                await context.Response.WriteAsJsonAsync(respuesta);
            }
        }
    }
}

