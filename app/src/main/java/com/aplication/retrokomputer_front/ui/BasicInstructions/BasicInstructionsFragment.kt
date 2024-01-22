package com.aplication.retrokomputer_front.ui.BasicInstructions


import android.content.Intent
import android.os.Bundle
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.fragment.app.Fragment
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.aplication.retrokomputer_front.R
import com.aplication.retrokomputer_front.databinding.FragmentBasicBinding

class BasicInstructionsFragment : Fragment() {

    private var _binding: FragmentBasicBinding? = null
    private lateinit var recyclerView: RecyclerView
    private lateinit var adapter: InstructionAdapter

    private val instructionsList = listOf(
        "Załaduj stałą do akumulatora", "Załaduj wartość z pamięci do akumulatora", "Inkrementacja wartości w pamięci", "Dekrementacja wartości w pamięci", "Dodawanie dwóch liczb i zapisanie wyniku w akumulatorze", "Dodawanie liczby z pamięci do akumulatora i zapisanie wyniku", "Odejmowanie jednej liczby od drugiej i zapisanie wyniku", "Porównanie dwóch liczb i ustawienie flag Z i N", "Arytmetyczne przesunięcie w lewo (mnożenie przez 2)", "Skok do adresu 0x0002", "Nieskończona pętla", "Inwersja bitów w rejestrze i zapisanie wyniku w akumulatorze", "Umieszczenie wartości na stosie i jej wyciągnięcie", "Zapisywanie i przywracanie wartości rejestrów za pomocą stosu", "Zamiana wartości między stosu a akumulatorem", "Użycie stosu do wymiany wartości między dwoma rejestrami", "Sprawdzenie parzystości liczby w pamięci", "Program do sprawdzenia, czy wartość w akumulatorze jest zerem", "Program do ustawiania i resetowania flagi Carry"
    )

    private val opcodeList = listOf(
        "0xA9, 0x42", "0xAD, 0x20", "0xA5, 0x10, 0xE6, 0x10", "0xAD, 0x10, 0xC6, 0x10, 0x8D, 0x10", "0xA9, 0x05, 0x69, 0x03", "0xA9, 0x03, 0x8D, 0x10, 0xAD, 0x10", "0xA9, 0x08, 0xE9, 0x03", "0xA9, 0x03, 0xC9, 0x06", "0xA9, 0x05, 0x0A", "0x4C, 0x02", "0x4C, 0x00", "0xA9, 0x0F, 0x35, 0x10", "0xA9, 0x42, 0x48, 0x68", "0xA9, 0x10, 0x48, 0xA0, 0x20, 0x48, 0x68, 0x68", "0xA9, 0x42, 0x48, 0x68, 0x48, 0x68", "0xA2, 0x0F, 0xA0, 0x07, 0x9A, 0xBA", "0xA5, 0x10, 0x29, 0x01, 0xF0, 0x03, 0x4C, 0x09", "0xA9, 0x00, 0xC5, 0x10, 0xD0, 0x03, 0x4C, 0x0C", "0x18, 0x38"
    )

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        Log.d("BasicInstructionsFragment", "onViewCreated")
        _binding = FragmentBasicBinding.inflate(inflater, container, false)
        val root = _binding?.root

        recyclerView = root?.findViewById(R.id.recyclerView) ?: RecyclerView(requireContext())
        adapter = InstructionAdapter(instructionsList) { position ->

            val selectedOpcode = opcodeList[position]
            val intent = Intent(requireContext(), InstructionDetailsActivity::class.java)
            intent.putExtra("OPCODE_KEY", selectedOpcode)
            startActivity(intent)
        }

        recyclerView.adapter = adapter
        recyclerView.layoutManager = LinearLayoutManager(context)

        return root
    }

    override fun onDestroyView() {
        super.onDestroyView()
        _binding = null
    }


}
