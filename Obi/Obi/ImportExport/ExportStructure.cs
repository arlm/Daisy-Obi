﻿using System;
using System.Xml;
using System.Collections.Generic;
using System.IO;

using urakawa.daisy;

namespace Obi.ImportExport
{
    
    public class ExportStructure 
    {

        private string m_ExportDirectory;
        private ObiPresentation m_Presentation;

        public ExportStructure(ObiPresentation presentation, string exportDirectory)
        {
            m_Presentation = presentation;
            m_ExportDirectory = exportDirectory;
    }

         public void CreateFileSet()
        {
            List<SectionNode> sectionsList = ((ObiRootNode)m_Presentation.RootNode).GetListOfAllSections();
            XmlDocument nccDocument = CreateNCCStubDocument();

            m_IdCounter = 0;
            
            //m_ExportedSectionCount = 0;
                        

            for (int i = 0; i < sectionsList.Count; i++)
            {
                try
                {
                    //if (m_MaxDepth < sectionsList[i].Level) m_MaxDepth = sectionsList[i].Level;
                    CreateElementsForSection(nccDocument, sectionsList[i], i);
                }
                catch (System.Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.ToString());
                }
            }


            // write ncc file
            WriteXmlDocumentToFile(nccDocument,
                Path.Combine(m_ExportDirectory, "ncc.html"));

        }


    private void CreateElementsForSection ( XmlDocument nccDocument, SectionNode section, int sectionIndex )
            {
                        string nccFileName = "ncc.html";
            XmlNode bodyNode = nccDocument.GetElementsByTagName ( "body" )[0];
            XmlNode headingNode = nccDocument.CreateElement ( null, "h" + section.Level.ToString (), bodyNode.NamespaceURI );
            if (sectionIndex == 0)
                {
                XmlDocumentHelper.CreateAppendXmlAttribute ( nccDocument, headingNode, "class", "title" );
                }
            else
                {
                    XmlDocumentHelper.CreateAppendXmlAttribute(nccDocument, headingNode, "class", "section");
                }
                string headingID = "h"+ IncrementID;
            XmlDocumentHelper.CreateAppendXmlAttribute(nccDocument, headingNode, "id", headingID);
        headingNode.AppendChild(nccDocument.CreateTextNode(section.Label)) ; 
            bodyNode.AppendChild ( headingNode );

                        bool isFirstPhrase = true;
            //EmptyNode adjustedPageNode = m_NextSectionPageAdjustmentDictionary[section];
            bool isPreviousNodeEmptyPage = false;
            
            for (int i = 0; i < section.PhraseChildCount ; i++)
                {//1

                        EmptyNode phrase = section.PhraseChild(i);
                
                if ((phrase is PhraseNode && phrase.Used)
                    || ( phrase is EmptyNode && phrase.Role_ == EmptyNode.Role.Page  &&  phrase.Used))
                    {//2
                    
                    string pageID = null;
                    XmlNode pageNode = null;
                    if (phrase.Role_ == EmptyNode.Role.Page)
                        {//3
                        string strClassVal = null;
                        // increment page counts and get page kind
                        switch (phrase.PageNumber.Kind)
                            {//4
                        case PageKind.Front:
                        //m_PageFrontCount++;
                        strClassVal = "page-front";
                        break;

                        case PageKind.Normal:
                        //m_PageNormalCount++;
                        //if (phrase.PageNumber.Number > m_MaxPageNormal) m_MaxPageNormal = phrase.PageNumber.Number;
                        strClassVal = "page-normal";
                        break;

                        case PageKind.Special:
                        //m_PageSpecialCount++;
                        strClassVal = "page-special";
                        break;

                            }//-4

                        XmlNode pageXmlNode= pageNode = nccDocument.CreateElement ( null, "span", bodyNode.NamespaceURI );
                        XmlDocumentHelper.CreateAppendXmlAttribute(nccDocument, pageNode, "class", strClassVal);
                        pageID = "p" + IncrementID;
                        XmlDocumentHelper.CreateAppendXmlAttribute(nccDocument, pageNode, "id", pageID);
                        pageXmlNode.AppendChild(nccDocument.CreateTextNode(phrase.PageNumber.ToString()));
                        bodyNode.AppendChild ( pageNode );

                        }//-3

                        // add anchor and href to ncc elements
                        //XmlNode anchorNode = nccDocument.CreateElement ( null, "a", bodyNode.NamespaceURI );

                        //if (isFirstPhrase)
                            //{
                            //headingNode.AppendChild ( anchorNode );
                            //CreateAppendXmlAttribute ( nccDocument, anchorNode, "href", smilFileName + "#" + txtID );
                            //anchorNode.AppendChild (
                                //nccDocument.CreateTextNode ( section.Label ) );
                            //}
                        //else if (pageNode != null)
                            //{

                            //pageNode.AppendChild ( anchorNode );
                            //CreateAppendXmlAttribute ( nccDocument, anchorNode, "href", smilFileName + "#" + txtID );

                            //anchorNode.AppendChild (
                                //nccDocument.CreateTextNode ( phrase.PageNumber.ToString () ) );

                            //}
                        //}

                    
                    isFirstPhrase = false;

                    if (phrase is EmptyNode && phrase.Role_ == EmptyNode.Role.Page)
                    {
                        isPreviousNodeEmptyPage = true;
                    }
                    else
                    {
                        isPreviousNodeEmptyPage = false;
                    }
                    
                } // for loop ends

            
                }
            }

        private int m_IdCounter;
        private string IncrementID { get { return (++m_IdCounter).ToString(); } }

        public XmlDocument CreateNCCStubDocument()
        {
            XmlDocument nccDocument = new XmlDocument();
            nccDocument.XmlResolver = null;

            nccDocument.CreateXmlDeclaration("1.0", "utf-8", null);
            nccDocument.AppendChild(nccDocument.CreateDocumentType("html",
                "-//W3C//DTD XHTML 1.0 Transitional//EN",
                "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd",
                null));

            XmlNode htmlNode = nccDocument.CreateElement(null,
                "html",
                "http://www.w3.org/1999/xhtml");

            nccDocument.AppendChild(htmlNode);


            XmlDocumentHelper.CreateAppendXmlAttribute(nccDocument, htmlNode, "lang", "en");
            XmlDocumentHelper.CreateAppendXmlAttribute(nccDocument, htmlNode, "xml:lang", "en");


            XmlNode headNode = nccDocument.CreateElement(null, "head", htmlNode.NamespaceURI);
            htmlNode.AppendChild(headNode);
            XmlNode bodyNode = nccDocument.CreateElement(null, "body", htmlNode.NamespaceURI);
            htmlNode.AppendChild(bodyNode);

            return nccDocument;
        }

        public void WriteXmlDocumentToFile(XmlDocument xmlDoc, string path)
        {
            XmlTextWriter writer = null;
            try
            {
                if (!File.Exists(path))
                {
                    File.Create(path).Close();
                }

                writer = new XmlTextWriter(path, null);
                writer.Formatting = Formatting.Indented;
                xmlDoc.Save(writer);
            }
            finally
            {
                writer.Close();
                writer = null;
            }
        }

            }
}
