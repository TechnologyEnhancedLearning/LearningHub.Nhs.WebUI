namespace LearningHub.Nhs.OpenApi.Repositories.Repositories
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// MultipleResultSets.
    /// </summary>
    public static class MultipleResultSets
    {
        /// <summary>
        /// MultipleResults extension.
        /// </summary>
        /// <param name="db">DbContext.</param>
        /// <param name="command">Store Proc.</param>
        /// <returns>MultipleResultSetWrapper.</returns>
        public static MultipleResultSetWrapper MultipleResults(this DbContext db, SqlCommand command)
        {
            return new MultipleResultSetWrapper(db, command);
        }

        /// <summary>
        /// MultipleResultSetWrapper.
        /// </summary>
        public class MultipleResultSetWrapper
        {
            private readonly DbContext dbContext;
            private readonly SqlCommand command;
            private List<Func<DbDataReader, IEnumerable>> resultSets;

            /// <summary>
            /// Initializes a new instance of the <see cref="MultipleResultSetWrapper"/> class.
            /// </summary>
            /// <param name="dbContext">DbContext.</param>
            /// <param name="storedProcedure">Store Proc.</param>
            public MultipleResultSetWrapper(DbContext dbContext, SqlCommand storedProcedure)
            {
                this.dbContext = dbContext;
                command = storedProcedure;
                resultSets = new List<Func<DbDataReader, IEnumerable>>();
            }

            /// <summary>
            /// With result.
            /// </summary>
            /// <typeparam name="TResult">Type.</typeparam>
            /// <returns>MultipleResultSetWrapper.</returns>
            public MultipleResultSetWrapper With<TResult>()
            {
                resultSets.Add(reader => MapToList<TResult>(reader));

                return this;
            }

            /// <summary>
            /// Execute.
            /// </summary>
            /// <returns>IEnumerable list.</returns>
            public List<IEnumerable> Execute()
            {
                var results = new List<IEnumerable>();

                var connection = dbContext.Database.GetDbConnection();

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                command.Connection = (SqlConnection)connection;
                using (var reader = command.ExecuteReader())
                {
                    foreach (var resultSet in resultSets)
                    {
                        results.Add(resultSet(reader));
                        reader.NextResult();
                    }
                }

                return results;
            }

            /// <summary>
            /// Retrieves the column values from the stored procedure and maps them to <typeparamref name="T"/>'s properties.
            /// </summary>
            /// <typeparam name="T">Type.</typeparam>
            /// <param name="dr">DbDataReader.</param>
            /// <returns>Array of type.</returns>
            private static List<T> MapToList<T>(DbDataReader dr)
            {
                var objList = new List<T>();
                var props = typeof(T).GetRuntimeProperties().ToList();

                var colMapping = dr.GetColumnSchema()
                    .Where(x => props.Any(y => y.Name.ToLower() == x.ColumnName.ToLower()))
                    .ToDictionary(key => key.ColumnName.ToLower());

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        T obj = Activator.CreateInstance<T>();
                        foreach (var prop in props)
                        {
                            if (colMapping.ContainsKey(prop.Name.ToLower()))
                            {
                                var column = colMapping[prop.Name.ToLower()];

                                if (column?.ColumnOrdinal != null)
                                {
                                    var val = dr.GetValue(column.ColumnOrdinal.Value);
                                    prop.SetValue(obj, val == DBNull.Value ? null : val);
                                }
                            }
                        }

                        objList.Add(obj);
                    }
                }

                return objList;
            }
        }
    }
}
