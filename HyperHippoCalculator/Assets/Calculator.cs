using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class Calculator : MonoBehaviour
{
	public enum mathType {add, sub, mult, div, NA};

	public Text mainText;
	public Text answerText;

	private string mathStr = "";
	private string resultString = "";

	private bool zeroDiv = false;
	// Use this for initialization
	void Start () 
	{
		mainText.text = "";
		answerText.text = "";
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public void Calculate()
	{

		string answer = "ERROR";
		try
		{
			answer = DoBracketMath(mathStr);
		}
		catch(Exception e)
		{
			Debug.Log("error " + e);
		}
		if(answer.Contains("(") || answer.Contains(")") || answer.Contains("+") || 
			answer.Contains("-") || answer.Contains("x") || answer.Contains("/") )
		{
			answer = "ERROR";
		}
		if(zeroDiv)
		{
			zeroDiv = false;
			answer = "DIVIDED BY ZERO UNIVERSE DESTROYED.";
		}
		resultString =  "= " + answer;
		answerText.text = resultString;

	}

	// new strategy! 
	/*
run through string to find brackets. use nested functions to dive into brackets until there 
are no more brackets to detect.
then run the "solve string of math" that will solve problems in bedmas order.
find multiplication problems first, do them, replace the problem with answer in the string.
then find division, and repeat until string can be parsed into float and/or no more math signs.
run the bracket finding algorithm the same but instead of a float return a solved string.
and replace the bracket block with that string.

	*/
	private string DoBracketMath(string subStr)
	{
		Debug.Log(subStr);
		//mathType myType = mathType.NA;
		string strnum1 = "", strnum2 = "";
		float num1 = 0, num2 = 0;
		string s;
		bool hasNum1 = false;
		//bool hasNum2 = false;  
		for(int i = 0 ; i < subStr.Length ; i++)
		{
			s = subStr.Substring(i, 1);
			//Debug.Log(s);
			if(s.Equals("("))
			{
				string tempStr = GetEnclosedBrackets(subStr.Substring(i));

				string bracketAns = DoBracketMath(tempStr);
				Debug.Log("bracketAns = " + bracketAns);

				bool foundlazyness = false;
				string front = "", back = "";
				if(i > 0)
				{
					front = subStr.Substring(0, i);
				}
				if(i + tempStr.Length + 2 < subStr.Length)
				{
					back = subStr.Substring(i + tempStr.Length + 2);
				}
				Debug.Log(front + "?" + back);
				if(!front.EndsWith("x") && !front.EndsWith("/") && !front.EndsWith("+") && 
					!front.EndsWith("-") && !front.EndsWith("x") && !front.Equals("") )
				{
					front += "x";
					foundlazyness = true;
				}
				if(!back.StartsWith("x") && !back.StartsWith("/") && !back.StartsWith("+") && 
					!back.StartsWith("-") && !back.StartsWith("x") && !back.Equals("") )
				{
					back = "x" + back;
					foundlazyness = true;
				}

				if(foundlazyness)
				{
					subStr = front + "(" + tempStr + ")" + back;
					Debug.Log("new lazyness substring = " + subStr);
				}

				subStr = subStr.Replace("(" + tempStr + ")", bracketAns); 
				Debug.Log("new substring = " + subStr);
				i = -1;
			}
		}

		string operation = "x";
		float resultf = 0;
		mathType currentType = mathType.mult;
		int infloopstopper = 0;
		int rstart = 0;
		while(!float.TryParse(subStr,out resultf) && infloopstopper < 40)
		{
			infloopstopper++;
			for(int i = 0 ; i < subStr.Length ; i++)
			{
				s = subStr.Substring(i,1);
				if( s.Equals("+") || s.Equals("-") || s.Equals("x") || s.Equals("/") )
				{
					//Debug.Log("found mathType");
					if( s.Equals(operation) && !hasNum1)
					{
						num1 = float.Parse(strnum1);

						hasNum1 = true;
					}
					else if(hasNum1)
					{
						num2 = float.Parse(strnum2);
						float tempans = DoMath(num1, num2, currentType);
						string oldstr = strnum1 + operation + strnum2;
						Debug.Log("oldstr (" + oldstr + ")");
						Debug.Log("mathing substr " + subStr);

						string front = "", back = "";
						if(rstart > 0)
						{
							front = subStr.Substring(0, rstart);
						}
						back = subStr.Substring(i);
						//subStr = subStr.Replace(oldstr, tempans.ToString());
						subStr = front + tempans.ToString() + back;
						Debug.Log("new Substr " + subStr);

						hasNum1 = false;
						strnum1 = "";
						strnum2 = "";
						i = -1;
					}
					else
					{
						strnum1 = "";
					}
				}
				else if( i + 1 == subStr.Length && hasNum1)
				{
					strnum2 += s;
					num2 = float.Parse(strnum2);
					float tempans = DoMath(num1, num2, currentType);
					string oldstr = strnum1 + operation + strnum2;
					Debug.Log("oldstr (" + oldstr + ")");
					Debug.Log("mathing substr " + subStr);

					string front = "";
					if(rstart > 0)
					{
						front = subStr.Substring(0, rstart);
					}

					//subStr = subStr.Replace(oldstr, tempans.ToString());
					subStr = front + tempans.ToString();
					Debug.Log("new Substr " + subStr);

					hasNum1 = false;
					strnum1 = "";
					strnum2 = "";
				}
				else if(hasNum1)
				{						
					strnum2 += s;
				}
				else
				{
					if(strnum1 == "")
					{
						rstart = i;
					}
					strnum1 += s;
				}
			}

			switch(currentType)
			{
			case mathType.mult:
				{
					if(!subStr.Contains(operation))
					{
						
						currentType = mathType.div;
						operation = "/";
					}
					break;
				}
			case mathType.div:
				{
					if(!subStr.Contains(operation))
					{
						currentType = mathType.add;
						operation = "+";
					}
					break;
				}
			case mathType.add:
				{
					if(!subStr.Contains(operation))
					{
						currentType = mathType.sub;
						operation = "-";
					}
					break;
				}
			}


			strnum1 = "";
			strnum2 = "";
			hasNum1 = false;

		}

		return subStr;
	}
		

	private float DoMath(float num1, float num2, mathType type)
	{
		Debug.Log("num1 = " + num1 + " num2 = " + num2 + "    " + type);
		float answer = 0;
		switch(type)
		{
		case mathType.add:
			{
				answer = num1 + num2;
				break;
			}
		case mathType.sub:
			{
				answer = num1 - num2;
				break;
			}
		case mathType.mult:
			{
				answer = num1 * num2;
				break;
			}
		case mathType.div:
			{
				if(num2 != 0)
				{
					answer = num1 / num2;
				}
				else
				{
					// find way to stop everything
					zeroDiv = true;
					answer = 1;
				}
				break;
			}
		}
		return answer;
	}
	private string GetEnclosedBrackets(string subString )
	{
//		startIndex++;
		Debug.Log("getEnclosed substring = " + subString);
//		Debug.Log(".length = " + mathStr.Length);

		int closingBrackets = 0;
		int b = 0;
		bool notfound = true;
		for(int i = 1; i < subString.Length && notfound; i++)
		{
			string s = subString.Substring(i, 1);
			if(s.Equals("("))
			{
				closingBrackets++;
			}
			if(s.Equals(")"))
			{
				if(closingBrackets > 0)
				{
					closingBrackets--;
				}
				else
				{
					b = i;
					notfound = false;
				}
			}

		}
		string tempStr = subString.Substring(1, b - 1);
		return tempStr;
	}
//	private float DoFloatParse(string* numString)
//	{
//		float f = 0;
//		f = float.Parse(numString);
//		numString = "";
//
//		return f;
//	}


	public void GetInput(string inp)
	{
		mathStr += inp;

		mainText.text = mathStr;
	}
	public void ClearAll()
	{
		mathStr = "";
		mainText.text = mathStr;
		answerText.text = "";
	}
	public void DeleteInput()
	{
		if(mathStr.Length > 0)
		{
			
			int length = mathStr.Length;
			string tempStr = mathStr.Substring(length -1, 1);
			int amt = 1;
			/*
			if(tempStr.Equals(" "))
			{
				amt = 3;
			} 
			*/
			mathStr = mathStr.Substring(0,length - amt);;
			mainText.text = mathStr;
		}
	}


}
