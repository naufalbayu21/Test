using UnityEngine;

public struct ZeroCounter {   
	int state;
    
	void Decrement() {
		state -= 1;
		state = Mathf.Clamp(state, 0, state);
	}

	void Increment() {
		state += 1;
	}

	public static ZeroCounter operator ++(ZeroCounter c) 
	{
		c.Increment();
		return c;
	} 

	public static ZeroCounter operator --(ZeroCounter c) 
	{
		c.Decrement();
		return c;
	}

	public static implicit operator bool(ZeroCounter c)
	{
		return c.state > 0;
	}

	public void Clear()
	{
		state = 0;
	}
}