using Api = AppTemplate.AuthApi.Models;
using Db = AppTemplate.AuthApi.Database.Models;

namespace AppTemplate.AuthApi.Extensions.Mappers;

public static class ApiModelMappers
{
    public static Db.AppUser ToDatabaseModel(this Api.SignUpRequest request) =>
        new()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = request.UserName,
            Email = request.Email
        };
}
