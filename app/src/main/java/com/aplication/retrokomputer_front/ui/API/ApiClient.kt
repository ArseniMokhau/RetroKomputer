package com.aplication.retrokomputer_front.ui.API
import EmulatorApiService
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory

object ApiClient {
    private const val BASE_URL = "http://10.0.2.2:5076/api/Emulator/" // Url Backend

    private val retrofit = Retrofit.Builder()
        .baseUrl(BASE_URL)
        .addConverterFactory(GsonConverterFactory.create())
        .build()

    val emulatorApiService: EmulatorApiService = retrofit.create(EmulatorApiService::class.java)


}


