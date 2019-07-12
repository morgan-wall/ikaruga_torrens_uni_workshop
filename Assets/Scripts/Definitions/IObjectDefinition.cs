using UnityEngine;

public interface IObjectDefinition<TObject>
    where TObject : MonoBehaviour
{
    TObject MakeObject();
}
