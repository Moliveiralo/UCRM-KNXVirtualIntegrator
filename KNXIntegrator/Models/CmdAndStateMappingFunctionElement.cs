using System;
using System.Collections.Generic;

namespace Models;
public class FunctionalModel {

    private int dpt_value;

    public FunctionalModel(int value){
        dpt_value = value;
    }

    public override string ToString(){
        return "DPT value = "+dpt_value.ToString()+"\n\n";
    }


}