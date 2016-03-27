using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class Calculator : MonoBehaviour
{
	public enum mathType {add, sub, mult, div, NA};

	public Text mainText;
	public Text answerText;

	private string mathStr = "";
	private string resultString = "";

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
		float answer = DoBracketMath(mathStr);
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
	private float DoBracketMath(string subStr)
	{
		Debug.Log(subStr);
		mathType myType = mathType.NA;
		string strnum1 = "", strnum2 = "";
		float num1 = 0, num2 = 0;
		string s;
		bool hasNum1 = false;
		//bool hasNum2 = false;
		for(int i = 0 ; i < subStr.Length ; i++)
		{
			s = subStr.Substring(i, 1);
			Debug.Log(s);
			if(s.Equals("(") || s.Equals(")") || s.Equals("+") || s.Equals("-") 
				|| s.Equals("x") || s.Equals("/") )
			{
//				Debug.Log("found blank at index " + i + " length = " + subStr.Length);	
//				s2 = subStr.Substring(i+1, 1);
				if(s.Equals("("))
				{
					int bracketEndIndex = 0;
					if(myType != mathType.NA )
					{
						string tempStr = GetEnclosedBrackets(subStr.Substring(i));
						bracketEndIndex = tempStr.Length + i + 1;
						Debug.Log( mathStr.Substring(bracketEndIndex - 1));
						num2 = DoBracketMath(tempStr);

						i = bracketEndIndex;

						float tempnum = DoMath(num1,num2,myType);
						num2 = 0;
						num1 = tempnum;
						myType = mathType.NA;
						strnum1 = num1.ToString();
						strnum2 = "";
						//hasNum1 = true;
						hasNum1 = true;
					}
					else
					{
						string tempStr = GetEnclosedBrackets(subStr.Substring(i));
						bracketEndIndex = tempStr.Length + i + 1;
						Debug.Log( mathStr.Substring(bracketEndIndex - 1));
						Debug.Log(bracketEndIndex); 
						num1 = DoBracketMath(tempStr);
						strnum1 = num1.ToString();
						i = bracketEndIndex;
						hasNum1 = true;
					}
				}
				else
				{
					Debug.Log("found mathType");

//					if(hasNum1 && strnum2 == "")
//					{
//						float tempnum = DoMath(num1,num2,myType);
//						num2 = 0;
//						num1 = tempnum;
//						myType = mathType.NA;
//					}
//					else
					if(myType != mathType.NA )
					{
						num2 = float.Parse(strnum2);
						strnum2 = "";
						float tempnum = DoMath(num1,num2,myType);
						num2 = 0;
						num1 = tempnum;
						myType = mathType.NA;
						hasNum1 = true;
					}
					else if(s.Equals("x"))
					{
						myType = mathType.mult;
						num1 = float.Parse(strnum1);
						strnum1 = "";
						hasNum1 = true;
					}
					else if(s.Equals("/"))
					{
						myType = mathType.div;
						num1 = float.Parse(strnum1);
						strnum1 = "";
						hasNum1 = true;
					}
					else if(s.Equals("+"))
					{
						myType = mathType.add;
						num1 = float.Parse(strnum1);
						strnum1 = "";
						hasNum1 = true;
					}
					else if(s.Equals("-"))
					{
						myType = mathType.sub;
						num1 = float.Parse(strnum1);
						strnum1 = "";
						hasNum1 = true;
					}
//					if(s.Equals("x"))
//					{
//						myType = mathType.mult;
//					}
//					else if(s.Equals("/"))
//					{
//						myType = mathType.div;
//					}
//					else if(s.Equals("+"))
//					{
//						myType = mathType.add;
//					}
//					else if(s.Equals("-"))
//					{
//						myType = mathType.sub;
//					}
						
				}
			}
			else
			{
				if(hasNum1)
				{
					strnum2 += s;
				}
				else
				{
					strnum1 += s;
				}
			}
		}
		float retAnswer = 0;
		if(myType == mathType.NA)
		{
			Debug.Log("only had first number");
			retAnswer = num1;
		}
		else
		{
			num2 = float.Parse(strnum2);
			strnum2 = "";
			retAnswer = DoMath(num1, num2, myType);
		}

		return retAnswer; 
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
					answer = 0;
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
		string tempStr = mathStr.Substring(1, b - 1);
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
