using Grasshopper.Kernel.Types;
using ViewObjects;

namespace ViewTo.RhinoGh.Goo
{

  public class GH_ViewObj : GH_Goo<IViewObj>
  {
    public GH_ViewObj()
    {
      Value = default;
    }

    public GH_ViewObj(IViewObj data)
    {
      Value = data;
    }

    public GH_ViewObj(GH_Goo<IViewObj> other)
    {
      Value = other.Value;
    }

    public override bool IsValid => Value != default;

    public override string TypeName => IsValid && !string.IsNullOrEmpty(Value.TypeName()) ? Value.TypeName() : "ViewObj";

    public override string TypeDescription => "A ViewTo Object";

    public override string ToString() => IsValid && !string.IsNullOrEmpty(Value.TypeName()) ? Value.TypeName() : "ViewObj";
    // Not sure what this does... 
    public override object ScriptVariable() => Value;
    // work out copy logic 
    public override IGH_Goo Duplicate() => new GH_ViewObj();
    // REVIEW how this works with GOO
    public override bool CastTo<Q>(ref Q target)
    {
      if (!(target is GH_ViewObj))
        return false;

      target = (Q)(object)new GH_ViewObj
      {
        Value = Value
      };
      return true;
    }

    // REVIEW how this works with GOO
    public override bool CastFrom(object source)
    {
      IViewObj obj = default;
      // var type = source.GetType();

      switch (source)
      {
        case IViewObj viewObj:
          obj = viewObj;
          break;
        case GH_ViewObj viewObjGH:
          obj = viewObjGH.Value;
          break;
        case GH_Goo<IViewObj> goo:
          obj = goo.Value;
          break;
      }
      Value = obj;
      return true;
    }

    // private ViewObj TryCreateEmpty(object source)
    // {
    //   switch (source)
    //   {
    //     case ViewStudy _:
    //       return new ViewStudy();
    //     case ViewCloud _:
    //       return new ViewCloud();
    //     case ContentBundle _:
    //       return new ContentBundle();
    //     case TargetContent _:
    //       return new TargetContent();
    //     case BlockerContent _:
    //       return new BlockerContent();
    //     case DesignContent _:
    //       return new DesignContent();
    //     case ViewerBundle _:
    //       return new ViewerBundle();
    //     default:
    //       return null;
    //   }
    // }

  }
}
