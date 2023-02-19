using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sales.WEB.Repositories.Web.Repositories;

namespace Sales.WEB.Repositories
{
    public interface IRepository
    {
        Task<HttpResponseWrapper<T>> Get<T>(string url);

        Task<HttpResponseWrapper<Object>> Post<T>(String url, T model);

        Task<HttpResponseWrapper<TResponse>> Post<T, TResponse>(string url, T model);
        Task<HttpResponseWrapper<Object>> Delete(string url);
        Task<HttpResponseWrapper<Object>> Put<T>(String url, T model);
        Task<HttpResponseWrapper<TResponse>> Put<T, TResponse>(String url, T model);
    }
}
