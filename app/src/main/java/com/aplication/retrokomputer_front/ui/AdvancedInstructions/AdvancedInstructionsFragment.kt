package com.aplication.retrokomputer_front.ui.AdvancedInstructions

import android.content.Intent
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.TextView
import androidx.fragment.app.Fragment
import androidx.lifecycle.ViewModelProvider
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.aplication.retrokomputer_front.R
import com.aplication.retrokomputer_front.databinding.FragmentAdvancedBinding
import com.aplication.retrokomputer_front.databinding.FragmentBasicBinding
import com.aplication.retrokomputer_front.ui.BasicInstructions.InstructionAdapter
import com.aplication.retrokomputer_front.ui.BasicInstructions.InstructionDetailsActivity

class AdvancedInstructionsFragment : Fragment() {

    private var _binding: FragmentAdvancedBinding? = null
    private lateinit var recyclerView: RecyclerView
    private lateinit var adapter: InstructionAdapter

    private val instructionsList = listOf(
        "Operacja Logicznego XOR",
        "Operacja Przesunięcia w Prawo",
        "Operacja Przesunięcia w Lewo",
        "Operacje Inkrementacji i Dekrementacji",
        "Wczytaj Wartość z Pamięci do Akumulatora",
        "Rotacja w Lewo",
        "Rotacja w Prawo",
        "Skok Jeżeli Przeniesienie Jest Wyzerowane",
        "Wczytaj i Zapisz Rejestr Indeksowy Y"
    )

    private val opcodeList = listOf(
        "0xA9, 0x0A, 0x49, 0x05",
        "0xA9, 0x0A, 0x6A",
        "0xA9, 0x05, 0x0A",
        "0xA9, 0x05, 0xA2, 0x03, 0xE6, 0x10, 0xE8, 0xE8, 0xE8",
        "0xA9, 0x05, 0x8D, 0x10, 0xAD, 0x10",
        "0xA9, 0x05, 0x2A",
        "0xA9, 0x05, 0x6A",
        "0xA9, 0x05, 0x18, 0x90, 0x03, 0xEA, 0x85, 0x00",
        "0xA0, 0x05, 0xA2, 0x03, 0x99, 0x10"
    )

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        _binding = FragmentAdvancedBinding.inflate(inflater, container, false)
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