using System;
using System.Collections.Generic;

namespace KNX_Virtual_Integrator.Model.Entities;
public class FunctionalModel:IEquatable<FunctionalModel> {

    private int dpt_value;

    public FunctionalModel(int value){
        dpt_value = value;
    }

    public override string ToString(){
        return dpt_value.ToString();
    }

    public override bool Equals(object? obj)
    {
        // Check if the obj is null or not of the expected type
        if (obj is FunctionalModel other)
        {
            return Equals(other);
        }

        // Use the strongly-typed Equals method for the actual comparison
        return false;
    }


    public bool Equals(FunctionalModel? other){
        if (other is null) return false;
        return dpt_value == other.dpt_value;
    }

    public override int GetHashCode(){
        return dpt_value.GetHashCode();
    }


}