using UnityEngine;

public class GravityController
{
    // The only instance of the class
    static GravityController s_Instance = null;

    // Constant gravity scale
    const float m_GravityScale = -20.0f;

    // Private constructor to stop accidental creation
    private GravityController()
    { }

    public static GravityController Instance()
    {
        // Creates an instance if there is not already one
        if (s_Instance == null)
        {
            s_Instance = new GravityController();
        }

        // Returns the instance
        return s_Instance;
    }

    public void SetScale(float scale)
    {
        // Sets the gravity
        Physics.gravity = new Vector3(0, m_GravityScale * scale, 0);
    }
}
