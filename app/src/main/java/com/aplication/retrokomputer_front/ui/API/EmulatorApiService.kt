import okhttp3.RequestBody
import okhttp3.ResponseBody
import retrofit2.Call
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.Headers
import retrofit2.http.POST

interface EmulatorApiService {
    @Headers("Content-Type: application/json")
    @POST("execute-program")
    fun executeProgram(@Body requestBody: RequestBody): Call<ResponseBody>

    @Headers("Content-Type: application/json")
    @POST("single-execute-program")
    fun singleExecuteProgram(@Body requestBody: RequestBody): Call<ResponseBody>

    @Headers("Content-Type: application/json")
    @GET("dump-memory")
    fun dumpMemory(): Call<ResponseBody>

    @Headers("Content-Type: application/json")
    @GET("dump-params")
    fun dumpParams(): Call<ResponseBody>

    @Headers("Content-Type: application/json")
    @POST("cancel-process")
    fun cancelProcess(): Call<ResponseBody>

    @Headers("Content-Type: application/json")
    @GET("clear-everything")
    fun clearEverything(): Call<ResponseBody>
}
