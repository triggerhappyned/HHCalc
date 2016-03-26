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



		/*
		string ansStr = "";
		string tempStr = "";
		float tempNum = 0;
		if(mathStr.Length > 0)
		{
			for(int i = 0 ; i < mathStr.Length; i++)
			{
				tempStr = mathStr.Substring(i,1);

				// if empty space then there is an operation sign, or bracket
				if(tempStr.Equals(" "))
				{
					tempStr = mathStr.Substring(i+1,1);

					if(tempStr.Equals("("))
					{


					}
				}
			}

		} */

	}
	private float DoBracketMath(string subStr)
	{
		mathType myType = mathType.NA;
		string strnum1 = "", strnum2 = "";
		float num1 = 0, num2 = 0;
		string s, s2;
		for(int i = 0 ; i < subStr.Length ; i++)
		{
			s = subStr.Substring(i, 1);
			if(s.Equals(" "))
			{
				s2 = subStr.Substring(i+1, 1);
				if(s2.Equals("("))
				{
					int bracketEndIndex = 0;
					if(myType == mathType.NA)
					{
						string tempStr = GetEnclosedBrackets(i+1);
						bracketEndIndex = tempStr.Length + i + 1;
						num1 = DoBracketMath(GetEnclosedBrackets(i+1));

						i = bracketEndIndex;
					}
					else
					{
						string tempStr = GetEnclosedBrackets(i+1);
						bracketEndIndex = tempStr.Length + i + 1;
						num2 = DoBracketMath(GetEnclosedBrackets(i+1));

						i = bracketEndIndex;
					}
				}
				else
				{
					if(s2.Equals("x"))
					{
						myType = mathType.mult;
						num1 = float.Parse(strnum1);
					}
					else if(s2.Equals("/"))
					{
						myType = mathType.div;
						num1 = float.Parse(strnum1);
					}
					else if(s2.Equals("+"))
					{
						myType = mathType.add;
						num1 = float.Parse(strnum1);
					}
					else if(s2.Equals("-"))
					{
						myType = mathType.sub;
						num1 = float.Parse(strnum1);
					}
				}
			}

			else
			{
				if(myType == mathType.NA)
				{
					strnum1 += s;
				}
				else
				{
					strnum2 += s;
				}
			}
		}

		return 0; //temp, change to get working

	}

	private float DoMath(float num1, float num2, mathType type)
	{
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
	private string GetEnclosedBrackets(int startIndex )
	{
		int openBrackets = 0;
		int closingBrackets = 0;
		int a = 0, b = 0;
		for(int i = startIndex + 1; i < mathStr.Length ; i++)
		{
			string s = mathStr.Substring(i, 1);
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
				}
			}

		}
		return mathStr.Substring(startIndex, b - startIndex);
	}
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
			if(tempStr.Equals(" "))
			{
				amt = 3;
			}
			mathStr = mathStr.Substring(0,length - amt);;
			mainText.text = mathStr;
		}
	}


}
