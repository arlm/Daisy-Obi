using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Forms;

namespace DTBDowngrader
    {
    public class Daisy3To202
        {
        string m_InputOpfPath;
        string m_OutputDirectory;
        DTBFilesInfo m_DAISY3Info;
        XmlDocument m_InputNcxDoc;
        XmlDocument m_NccDocument;
        Dictionary<int, XmlNode> m_PageListOpf;

        public Daisy3To202 ( string sourceOpf, string outputDirectory )
            {
            m_InputOpfPath = sourceOpf;
            m_OutputDirectory = outputDirectory;
            m_DAISY3Info = new DTBFilesInfo ( sourceOpf );
            m_InputNcxDoc = CommonFunctions.CreateXmlDocument ( m_DAISY3Info.NcxPath );
            m_NccDocument = CreateNCCSkeletonXmlDocument ();
            m_PageListOpf = new Dictionary<int, XmlNode> ();

            ExtractPageList ();
            CreateNccNavigationStructure ();
            CreateNCCMetadata ();

            WriteNCCFile ();
            }

        private void WriteNCCFile ()
            {
            string inputDir = Directory.GetParent ( m_InputOpfPath ).FullName;
            string nccPath = inputDir + "\\ncc.html";

            if (File.Exists ( nccPath ))
                File.Delete ( nccPath );

            File.Create ( nccPath ).Close ();
            CommonFunctions.WriteXmlDocumentToFile ( m_NccDocument, nccPath );
            }
        private XmlDocument CreateNCCSkeletonXmlDocument ()
            {
            System.Reflection.Assembly currentAssembly = System.Reflection.Assembly.GetExecutingAssembly ();
            Stream nccStream = currentAssembly.GetManifestResourceStream ( "DTBDowngrader.ncc.html" );


            XmlTextReader Reader = null;
            XmlDocument NccXmlDoc = null;
            // create xml reader and load xml document
            try
                {
                Reader = new XmlTextReader ( nccStream );
                Reader.XmlResolver = null;

                NccXmlDoc = new XmlDocument ();
                NccXmlDoc.XmlResolver = null;
                NccXmlDoc.Load ( Reader );
                }
            finally
                {
                Reader.Close ();
                Reader = null;
                }
            /*
                        try
                            {


                            //NccXmlDoc = CommonFunctions.CreateXmlDocument ( 
                                //Directory.GetParent (System.Reflection.Assembly.GetExecutingAssembly ().Location).FullName + "\\ncc.html" );
                            }
                        catch (System.Exception ex)
                            {
                            MessageBox.Show ( ex.ToString () );
                            }
             */
            return NccXmlDoc;
            }

        private void CreateNccNavigationStructure ()
            {

            TraverseNavMapAndCreateHeadings ( m_InputNcxDoc.GetElementsByTagName ( "navMap" )[0] );

            }

        private void TraverseNavMapAndCreateHeadings ( XmlNode node )
            {
            if (node.LocalName == "navPoint")
                {
                CreateHeadings ( node );
                }
            foreach (XmlNode n in node.ChildNodes)
                {
                TraverseNavMapAndCreateHeadings ( n );
                }
            }

        private void CreateHeadings ( XmlNode navPointNode )
            {
            XmlNode testNode = navPointNode.ParentNode;
            int i = 1;
            for (i = 1; i <= 6; i++)
                {
                if (testNode.LocalName == "navMap")
                    {
                    break;
                    }
                else
                    {
                    testNode = testNode.ParentNode;
                    }
                }

            //MessageBox.Show ( navPointNode.Attributes.GetNamedItem ( "playOrder" ).Value  + " : " + i.ToString () );

            string headingString = "h" + i.ToString ();
            string nSpace = m_NccDocument.GetElementsByTagName ( "html" )[0].NamespaceURI;
            XmlNode bodyNode = m_NccDocument.GetElementsByTagName ( "body" )[0];

            XmlNode headingNode = m_NccDocument.CreateElement ( null, headingString, nSpace );
            string headingClass = navPointNode == m_InputNcxDoc.GetElementsByTagName ( "navMap" )[0].FirstChild ? "title" : "section";
            CreateNccAttribute ( headingNode, "class", headingClass );
            CreateNccAttribute ( headingNode, "id",
                navPointNode.Attributes.GetNamedItem ( "id" ).Value );

            XmlNode anchorNode = m_NccDocument.CreateElement ( null, "a", nSpace );
            string smilRef = navPointNode.ChildNodes[1].Attributes.GetNamedItem ( "src" ).Value;
            CreateNccAttribute ( anchorNode, "href", smilRef );
            anchorNode.AppendChild ( m_NccDocument.CreateTextNode ( navPointNode.ChildNodes[0].FirstChild.InnerText ) );
            //MessageBox.Show ( smilRef );
            headingNode.AppendChild ( anchorNode );
            bodyNode.AppendChild ( headingNode );

            // check if next playOrder is of page, if so, add page
            int navPointPlayOrder = int.Parse ( navPointNode.Attributes.GetNamedItem ( "playOrder" ).Value );

            if (m_PageListOpf.ContainsKey ( navPointPlayOrder + 1 ))
                {
                AddPageToNCC ( m_PageListOpf[navPointPlayOrder + 1] );
                }
            }

        private void AddPageToNCC ( XmlNode pageNodeOpf )
            {
            string nSpace = m_NccDocument.GetElementsByTagName ( "html" )[0].NamespaceURI;
            XmlNode bodyNode = m_NccDocument.GetElementsByTagName ( "body" )[0];

            XmlNode pageNode = m_NccDocument.CreateElement ( null, "span", nSpace );

            string pageType = "page-" + pageNodeOpf.Attributes.GetNamedItem ( "type" ).Value;
            CreateNccAttribute ( pageNode, "class", pageType );
            CreateNccAttribute ( pageNode, "id",
                pageNodeOpf.Attributes.GetNamedItem ( "id" ).Value );

            XmlNode anchorNode = m_NccDocument.CreateElement ( null, "a", nSpace );
            string smilRef = pageNodeOpf.ChildNodes[1].Attributes.GetNamedItem ( "src" ).Value;
            CreateNccAttribute ( anchorNode, "href", smilRef );
            anchorNode.AppendChild ( m_NccDocument.CreateTextNode (
                pageNodeOpf.ChildNodes[0].FirstChild.InnerText ) );
            //MessageBox.Show ( smilRef );
            pageNode.AppendChild ( anchorNode );
            bodyNode.AppendChild ( pageNode );
            }

        private void ExtractPageList ()
            {
            XmlDocument ncxDocument = CommonFunctions.CreateXmlDocument ( m_DAISY3Info.NcxPath );

            XmlNode pageListNode = ncxDocument.GetElementsByTagName ( "pageList" )[0];

            foreach (XmlNode n in pageListNode.ChildNodes)
                {
                if (n.LocalName == "pageTarget")
                    {
                    m_PageListOpf.Add (
                        int.Parse ( n.Attributes.GetNamedItem ( "playOrder" ).Value ), n );
                    }
                }
            }



        private void CreateNCCMetadata ()
            {
            XmlNode headNode = m_NccDocument.GetElementsByTagName ( "head" )[0];

            XmlElement metaElement = m_NccDocument.CreateElement ( null, "meta", m_NccDocument.GetElementsByTagName ( "html" )[0].NamespaceURI );
            CreateNccAttribute ( metaElement, "content", "text/html; charset=utf-8" );
            CreateNccAttribute ( metaElement, "http-equiv", "Content-type" );
            headNode.AppendChild ( metaElement );

            XmlElement titleElement = m_NccDocument.CreateElement ( null, "title", m_NccDocument.GetElementsByTagName ( "html" )[0].NamespaceURI );
            titleElement.AppendChild ( m_NccDocument.CreateTextNode ( m_DAISY3Info.title ) );
            headNode.AppendChild ( titleElement );

            // create dc metadata
            foreach (string s in m_DAISY3Info.DCMetadata.Keys)
                {
                string content = m_DAISY3Info.DCMetadata[s];
                if (s == "dc:Format")
                    content = "Daisy 2.02";

                CreateNccMetadataNodes ( headNode, s, content );
                }

            // collect non duplicate  XMetadata from opf and ncx in a dictionary
            Dictionary<string, string> XMetaData = new Dictionary<string, string> ();

            foreach (string s in m_DAISY3Info.XMetaData.Keys)
                {
                XMetaData.Add ( s, m_DAISY3Info.XMetaData[s] );
                }

            foreach (string s in m_DAISY3Info.NcxMetaData.Keys)
                {
                if (!XMetaData.ContainsKey ( s ))
                    {
                    XMetaData.Add ( s, m_DAISY3Info.NcxMetaData[s] );
                    }
                }
            // remove uid
            if (XMetaData.ContainsKey ( "dtb:uid" ))
                {
                XMetaData.Remove ( "dtb:uid" );
                }

            if (XMetaData.ContainsKey ( "dtb:maxPageNumber" ))
                {
                XMetaData.Add ( "ncc:maxPageNormal", XMetaData["dtb:maxPageNumber"] );
                XMetaData.Remove ( "dtb:maxPageNumber" );
                }



            // add TOC items
            string TOCItems = m_InputNcxDoc.GetElementsByTagName ( "navPoint" ).Count.ToString ();
            XMetaData.Add ( "ncc:tocItems", TOCItems );

            // count  normal, front and special pages
            int frontPages = 0;
            int normalPages = 0;
            int specialPages = 0;
            foreach (XmlNode n in m_PageListOpf.Values)
                {
                string pageType = n.Attributes.GetNamedItem ( "type" ).Value;
                switch (pageType)
                    {
                case "front":
                frontPages++;
                break;

                case "normal":
                normalPages++;
                break;

                case "special":
                specialPages++;
                break;
                    }
                }

            XMetaData.Add ( "ncc:pageNormal", normalPages.ToString () );
            XMetaData.Add ( "ncc:pageFront", frontPages.ToString () );
            XMetaData.Add ( "ncc:pageSpecial", specialPages.ToString () );

            // add info about sidebar, prod notes and foot notes
            //XMetaData.Add ( "ncc:sidebars", "0" );
            //XMetaData.Add ( "ncc:prodnotes", "0" );
            //XMetaData.Add ( "ncc:footnotes", "0" );


            // create XMetadata
            foreach (string s in XMetaData.Keys)
                {
                CreateNccMetadataNodes ( headNode, s.Replace ( "dtb:", "ncc:" )
                    , XMetaData[s] );
                }

            //CommonFunctions.WriteXmlDocumentToFile ( m_NccDocument, 
            //System.AppDomain.CurrentDomain.BaseDirectory +  "\\1.xml" );
            System.Windows.Forms.MessageBox.Show ( "done" );
            }

        private XmlElement CreateNccMetadataNodes ( XmlNode headNode, string name, string content )
            {
            XmlElement metaElement = m_NccDocument.CreateElement ( null, "meta", m_NccDocument.GetElementsByTagName ( "html" )[0].NamespaceURI );
            CreateNccAttribute ( metaElement, "name", name );
            CreateNccAttribute ( metaElement, "content", content );

            headNode.AppendChild ( metaElement );
            return metaElement;
            }


        private XmlAttribute CreateNccAttribute ( XmlNode node, string attributeName, string attributeValue )
            {
            XmlAttribute att = m_NccDocument.CreateAttribute ( attributeName );
            att.Value = attributeValue;
            node.Attributes.Append ( att );
            return att;
            }




        }
    }
