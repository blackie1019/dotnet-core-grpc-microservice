# INFRASTRUCTURE Data #

## 功能說明  ##

1. 擴充四個呼叫MySql的方法, 會在呼叫StoreProcedure 前後插入Log 紀錄Performance
   - ExecuteReaderAsync (ADO.NET)
   - ExecuteNonQueryAsync (ADO.NET)
   - ExecuteQueryWithDapperAsync (Dapper)
   - ExecuteQuerySingleWithDapperAsync (Dapper)

### Rider Snippet ##

1. SPNonQueryAsync

```csharp
    public async Task $NAME$ ()
        {
            async Task Execute(MySqlConnection conn)
            {
                var parameters = GenerateParameters();
            
                var result = await conn.ExecuteNonQueryAsync
                (
                    spName,
                    parameters,
                    _transaction            
                );
                
                return result;
            }
            
            if(_testConnection == null)
            {
                using(var conn = new MySqlConnection(ConnString))
                {
                    await conn.OpenAsync();
                    await Execute(conn);
                }
            }
            else
            {
                    await Execute(_testConnection);
            }
            
            #region Parameters
            List<MySqlParameter> GenerateParameters()
            {
            var parameters = new List<MySqlParameter>();
                    
            return parameters;       
            }  
            #endregion
    }

```

2. SPQueryAsync

```csharp

    public async Task<$TYPE$> $NAME$ ()
    {
            async Task<$TYPE$> Execute(MySqlConnection conn)
            {
                var parameters = GenerateParameters();
            
                var result = await conn.ExecuteReaderAsync
                (
                    spName,
                    parameters,
                    _transaction,
                    ConvertCallBack
                );
                
                return result;
            }
            
            if(_testConnection == null)
            {
                using(var conn = new MySqlConnection(ConnString))
                {
                    await conn.OpenAsync();
                    return await Execute(conn);
                }
            }
            else
            {
                return await Execute(_testConnection);
            }
            
            #region ConvertCallBack
            async Task<$TYPE$> ConvertCallBack(DbDataReader dr)
            {
                await dr.ReadAsync();
                $TYPE$ result = ;//Please implatement 
                dr.Close();
                return result;
            }
            #endregion   
            
            #region Parameters
            List<MySqlParameter> GenerateParameters()
            {
            var parameters = new List<MySqlParameter>();
                    
            return parameters;       
            }  
            #endregion
    }

```

3. SPQuerySingleWithOrmAsync

```csharp
    public async Task<$TYPE$> $NAME$ ()
    {
            async Task<$TYPE$> Execute(MySqlConnection conn)
            {
                var parameters = GenerateParameters();
            
                var result = await conn.ExecuteQuerySingleWithOrmAsync
                (
                    spName,
                    parameters,
                    _transaction
                );
                
                return result;
            }
            
            if(_testConnection == null)
            {
                using(var conn = new MySqlConnection(ConnString))
                {
                    await conn.OpenAsync();
                    return await Execute(conn);
                }
            }
            else
            {
                return await Execute(_testConnection);
            }      
            
            #region Parameters
            DynamicParameters GenerateParameters()
            {
            var parameters = new DynamicParameters();
                    
            return parameters;       
            }  
            #endregion
    }

```

4. SPQueryWithOrmAsync

```csharp

    public async Task<IEnumerable<$TYPE$>> $NAME$ ()
    {
        async Task<IEnumerable<$TYPE$>> Execute(MySqlConnection conn)
        {
                var parameters = GenerateParameters();
            
                var result = await conn.ExecuteQueryWithOrmAsync
                (
                    spName,
                    parameters,
                    _transaction
                );
                
                return result;
            }
            
            if(_testConnection == null)
            {
                using(var conn = new MySqlConnection(ConnString))
                {
                    await conn.OpenAsync();
                    return await Execute(conn);
                }
            }
            else
            {
                return await Execute(_testConnection);
            }      
            
            #region Parameters
            DynamicParameters GenerateParameters()
            {
            var parameters = new DynamicParameters();
                    
            return parameters;       
            }  
            #endregion
    }

```