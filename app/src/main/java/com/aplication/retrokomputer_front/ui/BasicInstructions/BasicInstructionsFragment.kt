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
        "Wczytaj Natychmiastowe Wartości do Akumulatora i Rejestru X",
        "Zapisz Akumulator w Pamięci",
        "Dodaj Dwie Liczby",
        "Odjęcie Dwie Liczby",
        "Porównaj Dwie Wartości",
        "Skończ Jeżeli Nierówne",
        "Operacja Logicznego AND",
        "Operacja Logicznego OR"
    )

    private val opcodeList = listOf(
        "0xA9, 0x05, 0xA2, 0x03",
        "0xA9, 0x0A, 0x8D, 0x10",
        "0xA9, 0x05, 0x69, 0x03",
        "0xA9, 0x0A, 0xE9, 0x03",
        "0xA9, 0x0A, 0xC9, 0x0A",
        "0xA9, 0x05, 0xC9, 0x05, 0xB0, 0x03, 0x85, 0x00",
        "0xA9, 0x0F, 0x29, 0x05",
        "0xA9, 0x0A, 0x09, 0x05"
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
