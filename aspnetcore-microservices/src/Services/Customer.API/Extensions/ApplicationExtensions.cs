﻿namespace Customer.API.Extensions
{
    public static class ApplicationExtensions
    {
        public static void UseInfrastructure(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            //app.UseHttpsRedirection();

            app.UseAuthorization();
        }
    }
}
