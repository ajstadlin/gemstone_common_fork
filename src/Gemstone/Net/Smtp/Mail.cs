﻿//******************************************************************************************************
//  Mail.cs - Gbtc
//
//  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  12/30/2005 - Pinal C. Patel
//       Generated original version of source code.
//  12/12/2007 - Darrell Zuercher
//       Edited Code Comments.
//  09/22/2008 - J. Ritchie Carroll
//       Converted to C# - restructured.
//  10/15/2008 - Pinal C. Patel
//       Edited code comments.
//  09/14/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  09/02/2010 - Pinal C. Patel
//       Exposed underlying SmtpClient object via Client property for greater control.
//  12/01/2010 - Pinal C. Patel
//       Updated Send() method to parse To property value that could contain multiple email addresses.
//  12/14/2012 - Starlynn Danyelle Gilliam
//       Modified Header.
//
//******************************************************************************************************

// Ignore Spelling: Bcc username

using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Security;
using Gemstone.StringExtensions;

// ReSharper disable InconsistentNaming
namespace Gemstone.Net.Smtp;

/// <summary>
/// A wrapper class to the <see cref="MailMessage"/> class that simplifies sending mail messages.
/// </summary>
/// <example>
/// This example shows how to send an email message with attachment:
/// <code>
/// using System;
/// using Gemstone.Net.Smtp;
///
/// class Program
/// {
///     static void Main(string[] args)
///     {
///         Mail email = new Mail("sender@email.com", "recipient@email.com", "smtp.email.com");
///         email.Subject = "Test Message";
///         email.Body = "This is a test message.";
///         email.IsBodyHtml = true;
///         email.Attachments = @"c:\attachment.txt";
///         email.Send();
///         email.Dispose();
///
///         Console.ReadLine();
///     }
/// }
/// </code>
/// </example>
public class Mail : IDisposable
{
    #region [ Members ]

    // Constants
    /// <summary>
    /// Default <see cref="SmtpServer"/> to be used if one is not specified.
    /// </summary>
    public const string DefaultSmtpServer = "localhost";

    // Fields
    private string? m_from;
    private string? m_toRecipients;
    private string? m_username;
    private SecureString? m_password;
    private bool m_enableSSL;
    private bool m_disposed;

    #endregion

    #region [ Constructors ]

    /// <summary>
    /// Initializes a new instance of the <see cref="Mail"/> class.
    /// </summary>
    public Mail() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Mail"/> class.
    /// </summary>
    /// <param name="from">The e-mail address of the <see cref="Mail"/> message sender.</param>
    public Mail(string from) : this(from, "", DefaultSmtpServer) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Mail"/> class.
    /// </summary>
    /// <param name="from">The e-mail address of the <see cref="Mail"/> message sender.</param>
    /// <param name="toRecipients">A comma-separated or semicolon-separated e-mail address list of the <see cref="Mail"/> message recipients.</param>
    public Mail(string from, string toRecipients) : this(from, toRecipients, DefaultSmtpServer) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Mail"/> class.
    /// </summary>
    /// <param name="from">The e-mail address of the <see cref="Mail"/> message sender.</param>
    /// <param name="toRecipients">A comma-separated or semicolon-separated e-mail address list of the <see cref="Mail"/> message recipients.</param>
    /// <param name="smtpServer">The name or IP address of the SMTP server to be used for sending the <see cref="Mail"/> message.</param>
    public Mail(string from, string toRecipients, string smtpServer)
    {
        From = from;
        ToRecipients = toRecipients;
        SmtpServer = smtpServer;
    }

    /// <summary>
    /// Releases the unmanaged resources before the <see cref="Mail"/> object is reclaimed by <see cref="GC"/>.
    /// </summary>
    ~Mail()
    {
        Dispose(false);
    }

    #endregion

    #region [ Properties ]

    /// <summary>
    /// Gets or sets the e-mail address of the <see cref="Mail"/> message sender.
    /// </summary>
    /// <exception cref="ArgumentNullException">Value being assigned is a null or empty string.</exception>
    public string From
    {
        get
        {
            return m_from ?? string.Empty;
        }
        set
        {
            // This is a required field.
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));

            m_from = value;
        }
    }

    /// <summary>
    /// Gets or sets the comma-separated or semicolon-separated e-mail address list of the <see cref="Mail"/> message recipients.
    /// </summary>
    /// <exception cref="ArgumentNullException">Value being assigned is a null or empty string.</exception>
    public string ToRecipients
    {
        get
        {
            return m_toRecipients ?? string.Empty;
        }
        set
        {
            m_toRecipients = value;
        }
    }

    /// <summary>
    /// Gets or sets the comma-separated or semicolon-separated e-mail address list of the <see cref="Mail"/> message carbon copy (CC) recipients.
    /// </summary>
    public string? CcRecipients { get; set; }

    /// <summary>
    /// Gets or sets the comma-separated or semicolon-separated e-mail address list of the <see cref="Mail"/> message blank carbon copy (BCC) recipients.
    /// </summary>
    public string? BccRecipients { get; set; }

    /// <summary>
    /// Gets or sets the subject of the <see cref="Mail"/> message.
    /// </summary>
    public string? Subject { get; set; }

    /// <summary>
    /// Gets or sets the body of the <see cref="Mail"/> message.
    /// </summary>
    public string? Body { get; set; }

    /// <summary>
    /// Gets or sets the name or IP address of the SMTP server to be used for sending the <see cref="Mail"/> message.
    /// </summary>
    /// <exception cref="ArgumentNullException">Value being assigned is a null or empty string.</exception>
    public string SmtpServer
    {
        get
        {
            return Client?.Host ?? string.Empty;
        }
        set
        {
            // This is a required field.
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));

            // Dispose existing client.
            Client?.Dispose();

            // Instantiate new client.
            string[] split = value.Split(':');

            if (split.Length == 2 && int.TryParse(split[1], out int port))
                Client = new SmtpClient(split[0], port);
            else
                Client = new SmtpClient(value);

            ApplySecuritySettings();
        }
    }

    /// <summary>
    /// Gets or sets the comma-separated or semicolon-separated list of file names to be attached to the <see cref="Mail"/> message.
    /// </summary>
    public string? Attachments { get; set; }

    /// <summary>
    /// Gets or sets a boolean value that indicating whether the <see cref="Mail"/> message <see cref="Body"/> is to be formatted as HTML.
    /// </summary>
    public bool IsBodyHtml { get; set; }

    /// <summary>
    /// Gets or sets the username used to authenticate to the SMTP server.
    /// </summary>
    public string? UserName
    {
        get
        {
            return m_username;
        }
        set
        {
            m_username = value;
            ApplySecuritySettings();
        }
    }

    /// <summary>
    /// Gets or sets the password used to authenticate to the SMTP server.
    /// </summary>
    public string? Password
    {
        get
        {
            return m_password?.ToUnsecureString();
        }
        set
        {
            m_password = value?.ToSecureString();
            ApplySecuritySettings();
        }
    }

    /// <summary>
    /// Gets or sets the password used to authenticate to the SMTP server.
    /// </summary>
    public SecureString? SecurePassword
    {
        get
        {
            return m_password;
        }
        set
        {
            m_password = value;
            ApplySecuritySettings();
        }
    }

    /// <summary>
    /// Gets or sets the flag that determines whether
    /// to use SSL when communicating with the SMTP server.
    /// </summary>
    public bool EnableSSL
    {
        get
        {
            return m_enableSSL;
        }
        set
        {
            m_enableSSL = value;

            if (Client is not null)
                Client.EnableSsl = value;
        }
    }

    /// <summary>
    /// Gets the <see cref="SmtpClient"/> object used for sending the <see cref="Mail"/> message.
    /// </summary>
    public SmtpClient? Client { get; private set; }

    #endregion

    #region [ Methods ]

    /// <summary>
    /// Releases all the resources used by the <see cref="Mail"/> object.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="Mail"/> object and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (m_disposed)
            return;

        try
        {
            if (disposing)
                Client?.Dispose();
        }
        finally
        {
            m_disposed = true;  // Prevent duplicate dispose.
        }
    }

    /// <summary>
    /// Send the <see cref="Mail"/> message with <see cref="Attachments"/> to the <see cref="ToRecipients"/>, 
    /// <see cref="CcRecipients"/> and <see cref="BccRecipients"/> using the specified <see cref="SmtpServer"/>.
    /// </summary>
    public void Send()
    {
        MailMessage emailMessage = new()
        {
            From = new MailAddress(From),
            Subject = Subject,
            Body = Body,
            IsBodyHtml = IsBodyHtml
        };

        if (!string.IsNullOrEmpty(m_toRecipients))
        {
            // Add the specified To recipients for the mail message.
            foreach (string toRecipient in m_toRecipients.Split(';', ','))
                emailMessage.To.Add(toRecipient.Trim());
        }

        if (!string.IsNullOrEmpty(CcRecipients))
        {
            // Add the specified CC recipients for the mail message.
            foreach (string ccRecipient in CcRecipients.Split(';', ','))
                emailMessage.CC.Add(ccRecipient.Trim());
        }

        if (!string.IsNullOrEmpty(BccRecipients))
        {
            // Add the specified BCC recipients for the mail message.
            foreach (string bccRecipient in BccRecipients.Split(';', ','))
                emailMessage.Bcc.Add(bccRecipient.Trim());
        }

        if (!string.IsNullOrEmpty(Attachments))
        {
            // Attach the specified files to the mail message.
            foreach (string attachment in Attachments.Split(';', ','))
            {
                // Create the file attachment for the mail message.
                Attachment data = new(attachment.Trim(), MediaTypeNames.Application.Octet);
                ContentDisposition? header = data.ContentDisposition;

                if (header is not null)
                {
                    // Add time stamp information for the file.
                    header.CreationDate = File.GetCreationTime(attachment);
                    header.ModificationDate = File.GetLastWriteTime(attachment);
                    header.ReadDate = File.GetLastAccessTime(attachment);
                }

                emailMessage.Attachments.Add(data); // Attach the file.
            }
        }

        try
        {
            // Send the mail.
            Client?.Send(emailMessage);
        }
        finally
        {
            // Clean-up.
            emailMessage.Dispose();
        }
    }

    private void ApplySecuritySettings()
    {
        // If the SMTP client is null,
        // we cannot apply security settings
        if (Client is null)
            return;

        // Set the username and password used to authenticate to the SMTP server
        if (!string.IsNullOrEmpty(m_username) && m_password is not null)
            Client.Credentials = new NetworkCredential(m_username, m_password);
        else
            Client.Credentials = null;

        // Apply the flag to enable SSL
        Client.EnableSsl = m_enableSSL;
    }

    #endregion

    #region [ Static ]

    /// <summary>
    /// Sends a <see cref="Mail"/> message.
    /// </summary>
    /// <param name="from">The e-mail address of the <see cref="Mail"/> message sender.</param>
    /// <param name="toRecipients">A comma-separated or semicolon-separated e-mail address list of the <see cref="Mail"/> message recipients.</param>
    /// <param name="subject">The subject of the <see cref="Mail"/> message.</param>
    /// <param name="body">The body of the <see cref="Mail"/> message.</param>
    /// <param name="isBodyHtml">true if the <see cref="Mail"/> message body is to be formated as HTML; otherwise false.</param>
    /// <param name="smtpServer">The name or IP address of the SMTP server to be used for sending the <see cref="Mail"/> message.</param>
    public static void Send(string from, string toRecipients, string? subject, string? body, bool isBodyHtml, string smtpServer)
    {
        Send(from, toRecipients, null, null, subject, body, isBodyHtml, smtpServer);
    }

    /// <summary>
    /// Sends a <see cref="Mail"/> message.
    /// </summary>
    /// <param name="from">The e-mail address of the <see cref="Mail"/> message sender.</param>
    /// <param name="toRecipients">A comma-separated or semicolon-separated e-mail address list of the <see cref="Mail"/> message recipients.</param>
    /// <param name="ccRecipients">A comma-separated or semicolon-separated e-mail address list of the <see cref="Mail"/> message carbon copy (CC) recipients.</param>
    /// <param name="bccRecipients">A comma-separated or semicolon-separated e-mail address list of the <see cref="Mail"/> message blank carbon copy (BCC) recipients.</param>
    /// <param name="subject">The subject of the <see cref="Mail"/> message.</param>
    /// <param name="body">The body of the <see cref="Mail"/> message.</param>
    /// <param name="isBodyHtml">true if the <see cref="Mail"/> message body is to be formated as HTML; otherwise false.</param>
    /// <param name="smtpServer">The name or IP address of the SMTP server to be used for sending the <see cref="Mail"/> message.</param>
    public static void Send(string from, string toRecipients, string? ccRecipients, string? bccRecipients, string? subject, string? body, bool isBodyHtml, string smtpServer)
    {
        Send(from, toRecipients, ccRecipients, bccRecipients, subject, body, isBodyHtml, null, smtpServer);
    }

    /// <summary>
    /// Sends a <see cref="Mail"/> message.
    /// </summary>
    /// <param name="from">The e-mail address of the <see cref="Mail"/> message sender.</param>
    /// <param name="toRecipients">A comma-separated or semicolon-separated e-mail address list of the <see cref="Mail"/> message recipients.</param>
    /// <param name="subject">The subject of the <see cref="Mail"/> message.</param>
    /// <param name="body">The body of the <see cref="Mail"/> message.</param>
    /// <param name="isBodyHtml">true if the <see cref="Mail"/> message body is to be formated as HTML; otherwise false.</param>
    /// <param name="attachments">A comma-separated or semicolon-separated list of file names to be attached to the <see cref="Mail"/> message.</param>
    /// <param name="smtpServer">The name or IP address of the SMTP server to be used for sending the <see cref="Mail"/> message.</param>
    public static void Send(string from, string toRecipients, string? subject, string? body, bool isBodyHtml, string? attachments, string smtpServer)
    {
        Send(from, toRecipients, null, null, subject, body, isBodyHtml, attachments, smtpServer);
    }

    /// <summary>
    /// Sends a <see cref="Mail"/> message.
    /// </summary>
    /// <param name="from">The e-mail address of the <see cref="Mail"/> message sender.</param>
    /// <param name="toRecipients">A comma-separated or semicolon-separated e-mail address list of the <see cref="Mail"/> message recipients.</param>
    /// <param name="ccRecipients">A comma-separated or semicolon-separated e-mail address list of the <see cref="Mail"/> message carbon copy (CC) recipients.</param>
    /// <param name="bccRecipients">A comma-separated or semicolon-separated e-mail address list of the <see cref="Mail"/> message blank carbon copy (BCC) recipients.</param>
    /// <param name="subject">The subject of the <see cref="Mail"/> message.</param>
    /// <param name="body">The body of the <see cref="Mail"/> message.</param>
    /// <param name="isBodyHtml">true if the <see cref="Mail"/> message body is to be formated as HTML; otherwise false.</param>
    /// <param name="attachments">A comma-separated or semicolon-separated list of file names to be attached to the <see cref="Mail"/> message.</param>
    /// <param name="smtpServer">The name or IP address of the SMTP server to be used for sending the <see cref="Mail"/> message.</param>
    public static void Send(string from, string toRecipients, string? ccRecipients, string? bccRecipients, string? subject, string? body, bool isBodyHtml, string? attachments, string smtpServer)
    {
        using Mail email = new(from, toRecipients, smtpServer)
        {
            CcRecipients = ccRecipients,
            BccRecipients = bccRecipients,
            Subject = subject,
            Body = body,
            IsBodyHtml = isBodyHtml,
            Attachments = attachments
        };

        email.Send();
    }

    /// <summary>
    /// Sends a secure <see cref="Mail"/> message.
    /// </summary>
    /// <param name="from">The e-mail address of the <see cref="Mail"/> message sender.</param>
    /// <param name="toRecipients">A comma-separated or semicolon-separated e-mail address list of the <see cref="Mail"/> message recipients.</param>
    /// <param name="subject">The subject of the <see cref="Mail"/> message.</param>
    /// <param name="body">The body of the <see cref="Mail"/> message.</param>
    /// <param name="isBodyHtml">true if the <see cref="Mail"/> message body is to be formated as HTML; otherwise false.</param>
    /// <param name="smtpServer">The name or IP address of the SMTP server to be used for sending the <see cref="Mail"/> message.</param>
    /// <param name="username">The username of the account used to authenticate to the SMTP server.</param>
    /// <param name="password">The password of the account used to authenticate to the SMTP server.</param>
    public static void Send(string from, string toRecipients, string? subject, string? body, bool isBodyHtml, string smtpServer, string? username, string? password)
    {
        Send(from, toRecipients, subject, body, isBodyHtml, smtpServer, username, password, true);
    }

    /// <summary>
    /// Sends a secure <see cref="Mail"/> message.
    /// </summary>
    /// <param name="from">The e-mail address of the <see cref="Mail"/> message sender.</param>
    /// <param name="toRecipients">A comma-separated or semicolon-separated e-mail address list of the <see cref="Mail"/> message recipients.</param>
    /// <param name="subject">The subject of the <see cref="Mail"/> message.</param>
    /// <param name="body">The body of the <see cref="Mail"/> message.</param>
    /// <param name="isBodyHtml">true if the <see cref="Mail"/> message body is to be formated as HTML; otherwise false.</param>
    /// <param name="smtpServer">The name or IP address of the SMTP server to be used for sending the <see cref="Mail"/> message.</param>
    /// <param name="username">The username of the account used to authenticate to the SMTP server.</param>
    /// <param name="password">The password of the account used to authenticate to the SMTP server.</param>
    public static void Send(string from, string toRecipients, string? subject, string? body, bool isBodyHtml, string smtpServer, string? username, SecureString? password)
    {
        Send(from, toRecipients, subject, body, isBodyHtml, smtpServer, username, password, true);
    }

    /// <summary>
    /// Sends a secure <see cref="Mail"/> message.
    /// </summary>
    /// <param name="from">The e-mail address of the <see cref="Mail"/> message sender.</param>
    /// <param name="toRecipients">A comma-separated or semicolon-separated e-mail address list of the <see cref="Mail"/> message recipients.</param>
    /// <param name="subject">The subject of the <see cref="Mail"/> message.</param>
    /// <param name="body">The body of the <see cref="Mail"/> message.</param>
    /// <param name="isBodyHtml">true if the <see cref="Mail"/> message body is to be formated as HTML; otherwise false.</param>
    /// <param name="smtpServer">The name or IP address of the SMTP server to be used for sending the <see cref="Mail"/> message.</param>
    /// <param name="username">The username of the account used to authenticate to the SMTP server.</param>
    /// <param name="password">The password of the account used to authenticate to the SMTP server.</param>
    /// <param name="enableSSL">The flag that determines whether to use SSL when communicating with the SMTP server.</param>
    public static void Send(string from, string toRecipients, string? subject, string? body, bool isBodyHtml, string smtpServer, string? username, string? password, bool enableSSL)
    {
        Send(from, toRecipients, subject, body, isBodyHtml, smtpServer, username, password?.ToSecureString(),
            enableSSL);
    }

    /// <summary>
    /// Sends a secure <see cref="Mail"/> message.
    /// </summary>
    /// <param name="from">The e-mail address of the <see cref="Mail"/> message sender.</param>
    /// <param name="toRecipients">A comma-separated or semicolon-separated e-mail address list of the <see cref="Mail"/> message recipients.</param>
    /// <param name="subject">The subject of the <see cref="Mail"/> message.</param>
    /// <param name="body">The body of the <see cref="Mail"/> message.</param>
    /// <param name="isBodyHtml">true if the <see cref="Mail"/> message body is to be formated as HTML; otherwise false.</param>
    /// <param name="smtpServer">The name or IP address of the SMTP server to be used for sending the <see cref="Mail"/> message.</param>
    /// <param name="username">The username of the account used to authenticate to the SMTP server.</param>
    /// <param name="password">The password of the account used to authenticate to the SMTP server.</param>
    /// <param name="enableSSL">The flag that determines whether to use SSL when communicating with the SMTP server.</param>
    public static void Send(string from, string toRecipients, string? subject, string? body, bool isBodyHtml, string smtpServer, string? username, SecureString? password, bool enableSSL)
    {
        using Mail email = new(from, toRecipients, smtpServer)
        {
            Subject = subject,
            Body = body,
            IsBodyHtml = isBodyHtml,
            UserName = username,
            SecurePassword = password,
            EnableSSL = enableSSL
        };

        email.Send();
    }

    #endregion
}
