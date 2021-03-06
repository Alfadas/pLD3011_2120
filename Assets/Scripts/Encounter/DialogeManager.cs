﻿using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class DialogeManager : MonoBehaviour {
    [SerializeField] RollEncounter rollEncounter;
    [SerializeField] GameObject dialogeButtons;
    [SerializeField] Text headText;
    [SerializeField] Button button1;
    [SerializeField] Text button1Text;
    [SerializeField] Button button2;
    [SerializeField] Text button2Text;
    [SerializeField] Button button3;
    [SerializeField] Text button3Text;
    [SerializeField] Button button4;
    [SerializeField] Text button4Text;
    [SerializeField] Text[] buttonTexts;

    [SerializeField] string ok;
    [SerializeField] string nok;
    [SerializeField] string join;
    [SerializeField] string heal;

    List<XmlNode> answersNodes;

    public void StartDialoge(string dialoge)
    {
        ImportXml(dialoge);
        dialogeButtons.SetActive(true);
    }
    public void Button1()
    {
        if (button1Text.text == ok)
        {
            EndDialoge(ENCOUNTER_OUTCOME.End);
        }
        else if (button1Text.text == nok)
        {
            EndDialoge(ENCOUNTER_OUTCOME.Fight);
        }
        else if (button1Text.text == join)
        {
            EndDialoge(ENCOUNTER_OUTCOME.Join);
        }
        else if (button1Text.text == heal)
        {
            EndDialoge(ENCOUNTER_OUTCOME.Heal);
        }
        else if (button1Text.text != "")
        {
            ProcessDialogeStep(answersNodes[0]);
        }
    }
    public void Button2()
    {
        if (button2Text.text != "")
        {
            ProcessDialogeStep(answersNodes[1]);
        }
    }
    public void Button3()
    {
        if (button3Text.text != "")
        {
            ProcessDialogeStep(answersNodes[2]);
        }
    }
    public void Button4()
    {
        if (button4Text.text != "")
        {
            ProcessDialogeStep(answersNodes[3]);
        }
    }

    void EndDialoge(ENCOUNTER_OUTCOME proceed)
    {
        dialogeButtons.SetActive(false);
        rollEncounter.EndEncounter(proceed);
    }

    private void ImportXml(string name)
    {
        XmlDocument xmldoc = new XmlDocument();
        string path = Path.Combine(Application.streamingAssetsPath, name + ".xml").ToString();
        xmldoc.Load(path);
        XmlNode answersNode = xmldoc.FirstChild;
        ProcessDialogeStep(answersNode);
    }

    private void ProcessDialogeStep(XmlNode answersNode)
    {
        List<Answer> answers = new List<Answer>();
        if (answersNode.ChildNodes.Count > 1)
        {
            for (int i = 0; i < answersNode.ChildNodes.Count; i++)
            {
                string[] outcomeStrings = answersNode.ChildNodes[i].ChildNodes[2].InnerXml.Split(';');
                List<int> outcomes = new List<int>();
                foreach(string outcome in outcomeStrings)
                {
                    outcomes.Add(int.Parse(outcome));
                }
                Answer answer = new Answer(answersNode.ChildNodes[i].ChildNodes[0].InnerXml, int.Parse(answersNode.ChildNodes[i].ChildNodes[1].InnerXml), outcomes, answersNode.ChildNodes[i].ChildNodes[3]);
                answers.Add(answer);
            }
        }
        else
        {
            string[] outcomeStrings = answersNode.ChildNodes[0].ChildNodes[2].InnerXml.Split(';');
            List<int> outcomes = new List<int>();
            foreach (string outcome in outcomeStrings)
            {
                outcomes.Add(int.Parse(outcome));
            }
            Answer answer = new Answer(answersNode.ChildNodes[0].ChildNodes[0].InnerXml, int.Parse(answersNode.ChildNodes[0].ChildNodes[1].InnerXml), outcomes, answersNode.ChildNodes[0].ChildNodes[3]);
            answers.Add(answer);
        }

        Answer nextAnswer = answers[0];
        if (answers.Count > 1)
        {
            int roll = Random.Range(1, 101);
            int gesChance = 0;
            foreach (Answer answer in answers)
            {
                if (roll <= answer.GetChance())
                {
                    nextAnswer = answer;
                    break;
                }
                gesChance += answer.GetChance();
            }
        }

        headText.text = nextAnswer.GetAnswer();
        XmlNode questionsNode = nextAnswer.GetQuestionsNode();
        foreach (Text buttonText in buttonTexts)
        {
            buttonText.text = "";
        }
        if (nextAnswer.GetOutcome()[0] == 1)
        {
            buttonTexts[0].text = ok;
            return;
        }
        else if (nextAnswer.GetOutcome()[0] == 2)
        {
            buttonTexts[0].text = nok;
            return;
        }
        else if (nextAnswer.GetOutcome()[0] == 3)
        {
            buttonTexts[0].text = join;
            return;
        }
        else if (nextAnswer.GetOutcome()[0] == 4)
        {
            buttonTexts[0].text = heal;
            return;
        }
        else if (nextAnswer.GetOutcome()[0] > 4)
        {
            buttonTexts[0].text = ok;
            return;
        }

        answersNodes = new List<XmlNode>();
        if (questionsNode.ChildNodes.Count > 1 && questionsNode.ChildNodes.Count < 5)
        {
            for (int i = 0; i < questionsNode.ChildNodes.Count; i++)
            {
                buttonTexts[i].text = questionsNode.ChildNodes[i].ChildNodes[0].InnerXml;
                answersNodes.Add(questionsNode.ChildNodes[i].ChildNodes[1]);
            }
        }
        else
        {
            buttonTexts[0].text = questionsNode.ChildNodes[0].ChildNodes[0].InnerXml;
            answersNodes.Add(questionsNode.ChildNodes[0].ChildNodes[1]);
        }
    }
}