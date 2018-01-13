using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Http;
using Dapper;
using System.Net.Http;
using System.Net;
using System.Web.Http.Description;

namespace CarRentalSystem.Common
{
    public abstract class CustomApiController : ApiController
    {
        #region Fileds
        public static ILog _log;
        #endregion

        #region costructor
        public CustomApiController(Type classType)
        {
            Request = new HttpRequestMessage();
            _log = LogManager.GetLogger(classType);
        }

        #endregion

        #region Gets
        /// <summary>
        /// Get single value from stored procedure
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storedName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public T Get<T>(string storedName, object parameters = null) where T : class
        {
            T returnVal;
            using (IDbConnection db = new SqlConnection(Constants.AzureDatabase.ConnectionString))
            {
                returnVal = db.Query<T>(storedName, parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
            return returnVal;
        }

        /// <summary>
        /// Get multiple values from stored procedure
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storedName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public IEnumerable<T> GetMany<T>(string storedName, object parameters = null) where T : class
        {
            IEnumerable<T> returnVal;
            using (IDbConnection db = new SqlConnection(Constants.AzureDatabase.ConnectionString))
            {
                returnVal = db.Query<T>(storedName, parameters, commandType: CommandType.StoredProcedure);
            }
            return returnVal;
        }

        #endregion

        #region Execute Procedure
        /// <summary>
        /// Execute stored procedure
        /// </summary>
        /// <param name="storedName"></param>
        /// <param name="parameters"></param>
        [ApiExplorerSettings(IgnoreApi = true)]
        public void ExecuteStoredProcedure(string storedName, object parameters = null)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(Constants.AzureDatabase.ConnectionString))
                {
                    db.Execute(storedName, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception e)
            {
                string message = "An error occured.";
                _log.Error(message, e);
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(e.ParseEx()),
                    ReasonPhrase = message
                });
            }
        }


        /// <summary>
        /// Execute stored procedure
        /// </summary>
        /// <param name="storedName"></param>
        /// <param name="parameters"></param>
        [ApiExplorerSettings(IgnoreApi = true)]
        public HttpResponseMessage Execute(Action method)
        {
            try
            {
                method.Invoke();
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                string message = "An error occured.";
                _log.Error(message, e);

                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(e.ParseEx()),
                    ReasonPhrase = message
                });
            }
        }

        #endregion

    }
}