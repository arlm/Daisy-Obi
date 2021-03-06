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
        private int m_MaxDepth;
        private int m_PageNormalCount;

        public ExportStructure(ObiPresentation presentation, string exportDirectory)
        {
            m_Presentation = presentation;
            m_ExportDirectory = exportDirectory;
    }

        private bool m_Profile_VA= false;
        public bool Profile_VA
        {
            get { return m_Profile_VA; }
            set { m_Profile_VA = value; }
        }

         public void CreateFileSet()
        {
            List<SectionNode> sectionsList = ((ObiRootNode)m_Presentation.RootNode).GetListOfAllSections();
            XmlDocument nccDocument = CreateNCCStubDocument();
// add metadatas
            XmlNode bodyNode = nccDocument.GetElementsByTagName("body")[0];
            XmlNode paraNode = nccDocument.CreateElement("p", bodyNode.NamespaceURI);
            bodyNode.AppendChild(paraNode);
            XmlNode boldNode = nccDocument.CreateElement("b", bodyNode.NamespaceURI);
            paraNode.AppendChild(boldNode);
            boldNode.AppendChild(nccDocument.CreateTextNode("Book metadata:"));
            XmlNode ulNodeForMetadata = nccDocument.CreateElement("ul", bodyNode.NamespaceURI);
            paraNode.AppendChild(ulNodeForMetadata);
            List<string> metadataStringsList = new List<string>();
 

            m_IdCounter = 0;
            
            //m_ExportedSectionCount = 0;

            XmlNode prevPageXmlNode = null;
            m_MaxDepth = 0;
            m_PageNormalCount = 0;

            for (int i = 0; i < sectionsList.Count; i++)
            {
                try
                {
                    if (m_MaxDepth < sectionsList[i].Level) m_MaxDepth = sectionsList[i].Level;
                    //if (Profile_VA)
                    //{
                        //prevPageXmlNode = CreateElementsForSection(nccDocument, sectionsList[i], i, prevPageXmlNode);
                    //}
                    //else
                    //{
                        CreateElementsForSection(nccDocument, sectionsList[i], i, null);
                        
                    //}
                }
                catch (System.Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.ToString());
                }
            }

             // add metadata to top of the file
            urakawa.metadata.Metadata md = m_Presentation.GetFirstMetadataItem(Metadata.DC_TITLE);
            if (md != null) metadataStringsList.Add("Title: " + md.NameContentAttribute.Value);
            md = null;

            md = m_Presentation.GetFirstMetadataItem(Metadata.DC_IDENTIFIER);
            if (md != null) metadataStringsList.Add("Identifier: " + md.NameContentAttribute.Value);
            md = null;

            metadataStringsList.Add("Max. depth: " + m_MaxDepth.ToString());
            metadataStringsList.Add("Normal page count : " + m_PageNormalCount.ToString());

            foreach (string s in metadataStringsList)
            {
                XmlNode liNode = nccDocument.CreateElement("li", bodyNode.NamespaceURI);
                ulNodeForMetadata.AppendChild(liNode);
                liNode.AppendChild(nccDocument.CreateTextNode(s));
            }

            if (Profile_VA )
            {
                //XmlNode bodyNode = nccDocument.GetElementsByTagName("body")[0];
                int pageCounter = -1;
                List<XmlNode> nodesToRemove = new List<XmlNode>();

                XmlNode lastPageNode = null;
                XmlNode prevHeading = null;
                Dictionary<XmlNode, XmlNode> headingToNewPageMap = new Dictionary<XmlNode, XmlNode>();
                
                                for ( int i=0 ; i<bodyNode.ChildNodes.Count; i++)
                {//1
                    XmlNode childNode = bodyNode.ChildNodes[i];
                    if (childNode.Name == "h1" 
                        || childNode.Name == "h2"
                        || childNode.Name == "h3"
                        || childNode.Name == "h4"
                        || childNode.Name == "h5"
                        || childNode.Name == "h6")
                    {//2
                        if (lastPageNode != null )
                        {
                            XmlNode newPage = lastPageNode.Clone();
                            newPage.Attributes.GetNamedItem("id").Value = newPage.Attributes.GetNamedItem("id").Value + "_1";
                            
                            headingToNewPageMap.Add(childNode, newPage);
                            
                        }
                        pageCounter = 0;
                        prevHeading = childNode;
                        //Console.WriteLine("Heading has reset page counter: " + pageCounter);
                    }//-2
                    if (childNode.Name == "span")
                    {//2
                        lastPageNode = childNode;
                        //if (pageCounter > 0)
                        //{//3
                            //bodyNode.RemoveChild(childNode);
                            nodesToRemove.Add(childNode);
                            //Console.WriteLine("Removing span: " + childNode.InnerText);
                        //}//-3

                        pageCounter++;
                        Console.WriteLine("Incrementing page counter: " + pageCounter);
                    }//-2
                    //if (childNode.Name == "br" && pageCounter > 1) //counter is incremented above 
                    if (childNode.Name == "br" ) //counter is incremented above 
                    {//2
                        //bodyNode.RemoveChild(childNode);
                        nodesToRemove.Add(childNode);
                    }//-2
                }//-1
                // check if last heading has pages
                //if (pageCounter == 0 && lastPageNode != null && prevHeading != null)
                //{
                    //XmlNode newPage = lastPageNode.Clone();
                    //newPage.Attributes.GetNamedItem("id").Value = newPage.Attributes.GetNamedItem("id").Value + "_1";

                    //headingToNewPageMap.Add(prevHeading, newPage);
                //}

                // remove all XmlNodes collected for removal
                    if (nodesToRemove.Count > 0)
                    {
                        for (int i = 0; i < nodesToRemove.Count; i++)
                        {
                            bodyNode.RemoveChild(nodesToRemove[i]);
                        }
                    }
                // add new pages for the headings that do not have pages
                    foreach( XmlNode headingNode in headingToNewPageMap.Keys)
                    {   
                        bodyNode.InsertAfter(nccDocument.CreateElement("br"), headingNode) ;
                        bodyNode.InsertAfter(headingToNewPageMap[headingNode], headingNode);
                        Console.WriteLine("Adding to heading " + headingNode.InnerText );
                    }
            }

            // write ncc file
            WriteXmlDocumentToFile(nccDocument,
                Path.Combine(m_ExportDirectory, "Exported-Structure.html"));

        }


    private XmlNode CreateElementsForSection ( XmlDocument nccDocument, SectionNode section, int sectionIndex, XmlNode prevPageXmlNode )
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

            if (prevPageXmlNode != null)
            {
                XmlNode parent = prevPageXmlNode.ParentNode;
                parent.RemoveChild(prevPageXmlNode);

                bodyNode.AppendChild(prevPageXmlNode);
                bodyNode.AppendChild(nccDocument.CreateElement(null, "br", bodyNode.NamespaceURI));
                prevPageXmlNode = null;
                //Console.WriteLine("Prev: " + prevPageXmlNode.InnerText);
            }

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
                        m_PageNormalCount++;
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
                        string pageString = Profile_VA ? "Page " + phrase.PageNumber.ToString() :
                            phrase.PageNumber.ToString();
                        pageXmlNode.AppendChild(nccDocument.CreateTextNode(pageString));
                        bodyNode.AppendChild ( pageNode );
                        prevPageXmlNode = pageNode;
                        bodyNode.AppendChild( nccDocument.CreateElement(null, "br", bodyNode.NamespaceURI));
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
               // Console.WriteLine("returning : " + prevPageXmlNode.InnerText);
                return prevPageXmlNode;
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

            // add css file
                string cssFileName = CreateCSSFile();
                if (cssFileName != null)
                {
                    XmlNode linkNode = nccDocument.CreateElement("link", headNode.NamespaceURI);
                    XmlDocumentHelper.CreateAppendXmlAttribute(nccDocument, linkNode, "rel", "stylesheet");
                    XmlDocumentHelper.CreateAppendXmlAttribute(nccDocument, linkNode, "type", "text/css");
                    XmlDocumentHelper.CreateAppendXmlAttribute(nccDocument, linkNode, "href", cssFileName);
                    headNode.AppendChild(linkNode);
                }
            
            return nccDocument;
        }

        private string CreateCSSFile()
        {
            try
            {
                string cssFileName = "structure.css";
                string cssPath = Path.Combine(m_ExportDirectory, cssFileName);
                if (File.Exists(cssPath))
                {
                    File.Delete(cssPath);
                }
                if (Profile_VA)
                {
                    string sourcePath = Path.Combine(
                        Path.GetDirectoryName ( System.Reflection.Assembly.GetExecutingAssembly().Location),
                        "structure-VA.css");
                    File.Copy(sourcePath, cssPath, true);
                }
                else
                {
                    StreamWriter sw = File.CreateText(cssPath);

                    sw.WriteLine("h1{font-family:Times New Roman;font-size:21px;}");
                    sw.WriteLine("h2{font-family:Times New Roman;font-size:19px;}");
                    sw.WriteLine("h3{font-family:Times New Roman;font-size:17px;}");
                    sw.WriteLine("h4{font-family:Times New Roman;font-size:15px;}");
                    sw.WriteLine("h5{font-family:Times New Roman;font-size:13px;}");
                    sw.WriteLine("h6{font-family:Times New Roman;font-size:11px;}");
                    sw.Close();
                }
                return cssFileName;
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
            return null;
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
