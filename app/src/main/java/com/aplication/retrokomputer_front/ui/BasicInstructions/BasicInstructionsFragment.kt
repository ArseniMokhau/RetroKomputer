package com.aplication.retrokomputer_front.ui.BasicInstructions

import android.os.Bundle
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
        "LDX (Load X register with a constant)",
        "LDY (Load Y register with a constant)",
        "LDA (Load Accumulator with a constant)",
        "NOP (No Operation)",
        "INX (Increment X register)"
    )

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        _binding = FragmentBasicBinding.inflate(inflater, container, false)
        val root = _binding?.root

        recyclerView = root?.findViewById(R.id.recyclerView) ?: RecyclerView(requireContext()) // Обновлено здесь
        adapter = InstructionAdapter(instructionsList) { position ->
            // Обработка нажатия на элемент списка (вызов следующего фрагмента и т. д.)
            // Например, вы можете использовать Intent для открытия новой активности или фрагмента
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
