-- Cannot use an exists check as autoinsert's idenity
-- Created on 26/05/2020 for RapidResponse release, should only be run once.
UPDATE termsAndConditionsTBL SET active = 0 Where tenantId = 10 and active = 1;

INSERT INTO [dbo].[termsAndConditionsTBL]
           ([createdDate]
           ,[description]
           ,[details]
           ,[tenantId]
           ,[active]
           ,[reportable]
           ,[deleted]
           ,[amendUserID]
           ,[amendDate])
     VALUES
           (SYSDATETIMEOFFSET()
           ,'Learning Hub Terms and conditions-2023'
		   ,N'<h1 class="nhsuk-heading-xl nhsuk-u-padding-top-7">TERMS AND CONDITIONS</h1>
  <p>PLEASE READ THESE TERMS AND CONDITIONS CAREFULLY BEFORE USING THE PLATFORM. YOUR ATTENTION IS PARTICULARLY DRAWN TO THE PROVISIONS OF CLAUSE 14 (OUR RESPONSIBILITY FOR LOSS OR DAMAGE SUFFERED BY YOU) AND CLAUSE 15 (INDEMNITIES).</p>
  <h2 class="h2 nhsuk-heading-l">1. THE PLATFORM</h2>
  <p>1.1 These terms of use (<b>Terms</b>) set out the rules for using each of our following platforms (each being a <b>Platform</b>):</p>
  <p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">1.1.1 Elearning for healthcare hub (<b>elfh Hub</b>);</p>
  <p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">1.1.2 Digital Learning Solutions IT skills platform (<b>DLS</b>); and</p>
  <p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">1.1.3 NHS Learning Hub (<b>Learning Hub</b>).</p>
  <p>1.2 Each Platform is a collection of online learning (<b>elearning</b>) and educational resources (<b>Content</b>) which is:</p>
  <p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">1.2.1 <i>in respect of elfh Hub:</i> provided free of charge as part of our elearning for healthcare programme to support patient care by providing elearning to educate and train the health and social care workforce. The Content is developed by us in partnership with NHS, third sector and professional bodies;</pr>
  <p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">1.2.2 <i>in respect of DLS:</i> provided to aid registered health and social care organisations (<b>Centres</b>) with the training, development and assessment of competence of their staff. The Platform continues to support existing locally-developed learning;</p>
  <p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">1.2.3 <i>in respect of the Learning Hub:</i> provided free of charge to educate and train the health and social care workforce.  The Content is uploaded, shared and contributed by the Platform’s user community and by Technology Enhanced Learning (<b>TEL</b>) programme administrators .  Users can access, contribute, share and rate digital resources including video, audio, images, web links, documents and articles.</p>
  <p>1.3 By accessing a Platform, you warrant, represent and undertake that you are 18 years of age or older.</p>
  <p>1.4 Access to and usage of any Platform is granted in consideration of and is dependent on your acceptance of these Terms.</p>
  <h2 class="h2 nhsuk-heading-l">2. WHO WE ARE AND HOW TO CONTACT US</h2>
  <p>2.1 The Platforms are operated by NHS England (<b>we, us, our, NHSE</b>) via our Technology Enhanced Learning (<b>TEL</b>) programme. We are a Non-Departmental Public Body under the provisions of the Care Act 2014 and are responsible for the education, training and personnel development of the healthcare workforce for England. Our head offices are situated at NHS England London, Wellington house, 133-135 Waterloo Rd, London, SE1 8UG.</p>
  <p>2.2 To contact us about any matter relating to a Platform, please contact us as follows:</p>
  <p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">2.2.1 <i>in respect of elfh Hub:</i> email <a href="mailto:enquiries@e-lfh.org.uk">enquiries@e-lfh.org.uk</a>;</p>
  <p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">2.2.2 <i>in respect of DLS:</i></p>
  <p class="nhsuk-u-padding-left-7 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">2.2.2.1 if you are a Delegate (as defined in clause 10.1), support is provided through your centre manager. Please visit the Find Your Centre page on the Platform for contact details;</p>
  <p class="nhsuk-u-padding-left-7 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">2.2.2.2 if you are a Centre administrator, raise a support ticket using the process found within the Platform. If the tracking system is unavailable, Centre administrators can email <a href="mailto:support@dls.nhs.uk">support@dls.nhs.uk</a>;</p>
  <p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">2.2.3 <i>in respect of the Learning Hub:</i> email <a href="mailto:enquiries@learninghub.nhs.uk">enquiries@learninghub.nhs.uk</a>.</p>
  <p>2.3 If you find Content or links that you think should not be on a Platform or have any other feedback or concerns about the Content or functionality of a Platform, please report it to us as follows:</p>
  <p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">2.3.1 In respect of elfh Hub: using the functionality for reporting Content available via the Platform;</p>
  <p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">2.3.2 In respect of DLS and the Learning Hub: using the contact details provided in clause 2.2.</p>
  <h2 class="h2 nhsuk-heading-l">3. THESE TERMS</h2>
  <p>3.1 By using any Platform, you confirm that you accept these Terms insofar as they apply to that Platform and that you agree to comply with them. If you do not agree to these Terms insofar as they apply to any Platform, you must not use that Platform.</p>
  <p>3.2 These Terms incorporate the following which also apply to your use of any Platform:</p>
  <p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">3.2.1 our <a href="/policies/privacy-policy" target="_blank">Privacy Policy</a> (see further under clause 12); and</p>
  <p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">3.2.2 our <a href="/policies/acceptable-use-policy" target="_blank">Acceptable Use Policy</a> with which you agree to comply with at all times.</p>
  <p>3.3 We may amend these Terms from time to time without notice. Any change will be effective in respect of a Platform immediately upon the revised Terms being posted on that Platform. Every time you wish to use a Platform, please check these Terms to ensure you understand the Terms that apply at that time. You must stop using a Platform and deactivate your Account (as defined in clause 4) if you do not agree to such change. Continued use of a Platform after a change has been made is your acceptance of the change. These Terms were most recently updated on ' + convert(VARCHAR, getdate(), 106) + '.</p>
<h2 class="h2 nhsuk-heading-l">4. ACCESS TO A PLATFORM</h2>
<p>4.1 This clause 4 sets out the circumstances in which you may be granted a user account for access to a Platform (<strong>Account</strong>).</p>
<p>4.2 Access to elfh Hub is permitted in accordance with the following provisions:</p>
<p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">4.2.1 The Platform is a "closed" platform and can only be accessed by members of the health and social care workforce and other user groups as determined by our relevant policies. We will (at our sole discretion) determine whether you may be granted an Account.</p>
<p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">4.2.2 You are required to provide accurate and complete information, which may be obtained via your OpenAthens accreditation, or if you do not have OpenAthens accreditation you will be required to provide us with such information and materials as we may require in order to proceed to establish your credentials to our satisfaction. We will assess the information provided by you and confirm whether you are eligible for an Account. For the avoidance of doubt, we are under no obligation to grant an Account to any person.</p>
<p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">4.2.3 The extent of your access to the Platform will be determined by us, acting reasonably, in accordance with our internal Platform access protocol.</p>
<p>4.3 Access to the DLS is permitted in accordance with the following provisions:</p>
<p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">4.3.1 The Platform is a "closed" platform and can only be accessed by members of the health and social care workforce.</p>
<p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">4.3.2 If you are a Centre, we will (at our sole discretion) determine whether you may be granted an Account.</p>
<p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">4.3.3 If you are a Delegate (as defined in clause 10.1), whether you may be granted an Account is at the discretion of your Centre.</p>
<p>4.4 Access to the Learning Hub is permitted in accordance with the following provisions:</p>
<p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">4.4.1 The Platform is a "closed" platform and can only be accessed by members of the health and social care workforce. We will (at our sole discretion) determine whether you may be granted an Account.</p>
<p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">4.4.2 You are required to provide accurate and complete information, which may be obtained via your OpenAthens accreditation, or if you do not have OpenAthens accreditation you will be required to provide us with such information and materials as we may require in order to proceed to establish your credentials to our satisfaction. We will assess the information provided by you and confirm whether you are eligible for an Account. For the avoidance of doubt, we are under no obligation to grant an Account to any person.</p>
<h2 class="h2 nhsuk-heading-l">5. YOUR ACCOUNT</h2>
<p>5.1 When you set up your Account you must identify yourself honestly, accurately and completely. It is your responsibility to make sure that your Account details are correct and up to date. You must only access as Platform using your own username and password.</p>
<p>5.2 If you choose, or you are provided with, an Account code, password or any other piece of information (Account Information) as part of our security procedures, you must treat the Account Information as confidential. You must never input any of your Account Information into any other website, and we will never ask for you for your Account Information. Your Account is non-transferable and you must not disclose your Account Information to any third party or permit another person to complete any elearning on your behalf or access or upload any Content on your behalf.</p>
<p>5.3 We have the right to disable your Account code or password, whether chosen by you or allocated by us, at any time, if in our reasonable opinion you have failed to comply with any of these Terms.</p>
<p>5.4 You must not to use any account other than your own Account or permit or offer your Account to be used by any other person.</p>
<p>5.5 You are responsible for the security of your Account. If you know or suspect that anyone other than you knows your Account code or password, or that anyone other than you has access to your Account, you must promptly notify us using the contact details provided in clause 2.2.</p>
<p>5.6 You are solely responsible for all activity that occurs on your Account, including any information associated with your Account and anything that happens in relation to it including (without limitation) unauthorised or wilfully malicious access granted by you to another person.</p>
<p>5.7 If we have to contact you, we will do so by writing to you at the email address you provided to us when setting up your Account. Therefore, it is important to keep your account detail up to date, as set out in clause 5.1.</p>
<h2 class="h2 nhsuk-heading-l">6. USE OF A PLATFORM</h2>
<p>6.1 You may only use a Platform in accordance with our Acceptable Use Policy.</p>
<p>6.2 The Platforms and their Content are only targeted to, and intended for use by, people residing in the United Kingdom or British Overseas Territories (Permitted Territories). We do not represent that Content available on or through any Platform is appropriate for use or available in other locations. By continuing to access, view or make use of a Platform, you hereby warrant and represent to us that you are located in a Permitted Territory. If you are not located in a Permitted Territory, you must immediately discontinue use of the Platforms.</p>
<p>6.3 Save in respect of elfh Hub and Content on the Learning Hub, the Platforms and Content are provided ''as is''. We make no representation or endorsement of satisfactory quality, fitness for a particular purpose, non-infringement, compatibility, security, and accuracy of the Content.</p>
<p>6.4 While reasonable professional care has been taken in developing the Content (excluding any Third Party Content (as defined in clause 8.1) and any Contributions (as defined in clause 7.1)), we make no warranties concerning the correctness or completeness of the Content. Although we make reasonable efforts to update the Content, we make no representations, warranties, or guarantees, whether express or implied, that the Content is up to date.</p>
<h2 class="h2 nhsuk-heading-l">7. UPLOADING CONTENT TO A PLATFORM</h2>
<p>7.1 Whenever you make use of a feature that allows you to upload or contribute Content to a Platform (<strong>Contribution</strong>), you must comply with the Content Standards (as defined in paragraph 4 of our Acceptable Use Policy).</p>
<p>7.2 By uploading a Contribution to a Platform, you:</p>
<p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">7.2.1 Warrant and represent that such Contribution complies with the Content Standards (Link to Content standards) and that you have all rights, power, and authority necessary or desirable to grant the rights, use, and access to such Contribution in accordance with these Terms; and</p> 
<p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">7.2.2 Acknowledge that the Platform and any content contributed or linked from it is not intended to generate income and does not have the purpose of generating income.</p> 
<p>7.3 A Centre shall:</p>
<p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">7.3.1 Retain ownership of all intellectual property rights in any Content uploaded to a Platform by that Centre; and</p>
<p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">7.3.2 Grant us, or procure the direct grant to us, for the purpose of hosting the Platform, a fully paid-up, worldwide, non-exclusive, royalty-free, perpetual, and irrevocable license to copy Content uploaded to a Platform by that Centre.</p>
<p>7.4 In respect of the Learning Hub, when you submit a Contribution to the Platform, you must select the appropriate license in accordance with the guidance provided at <a href="https://support.learninghub.nhs.uk/support/solutions/articles/80000986606-which-licence-should-i-select-when-contributing-a-resource-to-the-learning-hub-" target="_blank">https://support.learninghub.nhs.uk/support/solutions/articles/80000986606-which-licence-should-i-select-when-contributing-a-resource-to-the-learning-hub-</a>. Whichever license you select, you grant the following rights in relation to that Contribution:</p>
<p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">7.4.1 A worldwide, non-exclusive, royalty-free, transferable license to us to use and display that Contribution on the Platform, to expire when you delete the Contribution from the Platform; and</p>
<p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">7.4.2 A worldwide, non-exclusive, royalty-free, perpetual, transferable license for users of the Platform to use the Contribution in accordance with the functionality of the Platform.</p>
<p>7.5 You are solely responsible for securing and backing up your Contributions.</p>
<p>7.6 We take no responsibility for and do not review nor expressly or implicitly endorse Contributions. Without prejudice to the foregoing, we do not endorse any Content or any link to any Content:</p>
<p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">7.6.1 Which is created for advertising, promotional, or other commercial purposes, including links, logos, and business names;</p>
<p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">7.6.2 Which requires a subscription or payment to gain access to such Content;</p>
<p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">7.6.3 In which the user has a commercial interest;</p>
<p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">7.6.4 Which promotes a business name and/or logo;</p>
<p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">7.6.5 Which contains a link to an app via iOS or Google Play; or</p>
<p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">7.6.6 Which has as its purpose or effect the collection and sharing of personal data.</p>
<p>7.7 We have the right to take action, as set out in paragraph 10 of our Acceptable Use Policy, including but not limited to removal of any Contribution, if, in our opinion, a Contribution does not comply with our Acceptable Use Policy.</p>
<h2 class="h2 nhsuk-heading-l">8. THIRD PARTY CONTENT</h2>
<p>8.1 The Platforms may contain a variety of Content including but not limited to audio files, videos, text, images, articles, links to other websites, facilities, products, services, resources, elearning and other links of any kind whatsoever and wherever in the world and other materials which may be posted from time to time by other users or any other third party (<strong>Third Party Content</strong>).</p>
<p>8.2 Third Party Content is not owned by us and is not under our influence or control in any respect whatsoever and has not been verified or approved by us. We assume no responsibility for any Third Party Content and do not endorse any Content or any link to any Content, including but not limited to Content of any type listed in clause 7.6 above. The appearance of Third Party Content on a Platform should not be interpreted as approval by us of those links or information you may obtain from such Third Party Content. We do not give any warranty as to the accuracy of Third Party Content. The views expressed by other users on a Platform do not represent our views or values.</p>
<p>8.3 We cannot guarantee that Third Party Content will work all of the time and have no control over the availability of any Third Party Content.</p>
<p>8.4 You use and access Third Party Content strictly at your own risk and you must exercise all care and due diligence on your own behalf before proceeding to pay any attention to, rely on or take any action in connection with Third Party Content.</p>
<h2 class="h2 nhsuk-heading-l">9. CHANGES TO, AND AVAILABILITY OF, A PLATFORM</h2>
<p>9.1 We reserve the right to update, move or change a Platform at any time in order to meet our users’ needs and our business priorities and to continually improve our online service. External web sites link to a Platform at their own risk.</p>
<p>9.2 We reserve the right to modify, suspend or discontinue a Platform at any time without notice. You agree that we will not be liable to you or to any third party for any modification, suspension or discontinuation of a Platform.</p>
<p>9.3 We do not guarantee that a Platform, or any Content on it, will always be available or be uninterrupted. We may suspend or withdraw or restrict the availability of all or any part of a Platform for business and operational reasons, including (without limitation) for technical or security reasons. We will try to give you reasonable notice of any suspension or withdrawal but are under no obligation to do so.</p>
<p>9.4 We do not warrant that the functions contained in the material contained on a Platform will be uninterrupted or error free, that defects will be corrected, or that a Platform or the server that makes it available are free of viruses or represent the full functionality, accuracy, reliability of any materials provided on a Platform.</p>
<p>9.5 Access to a Platform requires the functioning of servers and internet connections. We undertake to provide a service at least meeting elearning industry norms for those elements of availability which are under our control. We do not undertake to provide availability to the Content at any specific time.</p>
<h2 class="h2 nhsuk-heading-l">10. ACCESS VIA CENTRE (DLS ONLY)</h2>
<p>10.1 Where you are a Centre that provides access for your staff (Delegates) to a Platform, then:</p>
<p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">10.1.1 You must:</p>
<p class="nhsuk-u-padding-left-7 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">10.1.1.1 Provide appropriate support to Delegates in relation to their use of the Platform. Under no circumstances should your staff contact us directly to resolve issues or for advice relating to the Platform;</p>
<p class="nhsuk-u-padding-left-7 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">10.1.1.2 Ensure that Delegates understand where complaints in respect of the Platform should be directed within the Centre;</p>
<p class="nhsuk-u-padding-left-7 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">10.1.1.3 Make all reasonable attempts to resolve issues before escalating to us; and</p>
<p class="nhsuk-u-padding-left-7 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">10.1.1.4 Where any Delegate is identified as failing to comply with our Acceptable Use Policy, withdraw access to the Platform from the Delegate by inactivating the Delegate’s Account and notify us by raising a support ticket.</p>
<p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">10.1.2 We shall assume that all communication sent by Centre administrators through a Platform is official correspondence from you acting in your official capacity on behalf of your Centre.</p>
<p>10.2 We provide Platform support to Centres Monday to Friday 9am to 5pm excluding English bank holidays. A reduced service is provided over each Christmas holiday period and we will notify Centres of this in advance. We do not provide direct support to Delegates.</p>
<p>10.3 We will have full, unrestricted access to Content to enable support to be carried out in a speedy and efficient manner.</p>
<p>10.4 We undertake to respond to support tickets issued by Centres within 2 (two) working days, with one of the following responses:</p>
<p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">10.4.1 The issue is now resolved;</p>
<p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">10.4.2 More information from the Centre is required;</p>
<p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">10.4.3 The issue requires software development; or</p>
<p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">10.4.4 A request for change needs to be submitted and considered.</p>
<p>10.5 If a Platform is decommissioned and we hold any Content which was created by a Centre and/or any data which relates to elearning completed by the Delegates of a specific Centre (Data), we will provide the Data to that Centre. If you require the Data to be provided in a specific format, you must discuss this with us to determine (acting reasonably) whether this is feasible. The Data will be transferred securely and with send and receive receipts.</p>
<p>10.6 Where you are a Delegate, in the event that your Centre ceases to have access to a Platform, your access to a Platform may become limited or cease.</p>
<h2 class="h2 nhsuk-heading-l">11. WE ARE NOT RESPONSIBLE FOR VIRUSES</h2>
<p>We do not guarantee that a Platform will be secure or free from bugs or viruses. You are responsible for configuring your information technology and computer programmes to access a Platform. You should use your own virus protection software.</p>
<h2 class="h2 nhsuk-heading-l">12. HOW WE MAY USE YOUR PERSONAL INFORMATION</h2>
<p>We will only use your personal information as set out in our Privacy Policy (please see clause 3.2.1).</p>
<h2>13. INTELLECTUAL PROPERTY</h2>
<p>13.1 Without prejudice to clauses 7.3 and 7.4, we are the owner or the licensee of all intellectual property rights in a Platform, and in the material published on it. Those works are protected by copyright laws and treaties around the world. All such rights are reserved.</p>
<p>13.2 Save as expressly set out in these Terms, you shall not acquire in any way, any title, rights of ownership, or intellectual property rights of whatever nature in a Platform, its software or any Content or in any copies of it.</p>
<p>13.3 You acknowledge and understand that a Platform and its software and Content contains confidential and proprietary information. You shall not conceal, modify, remove, destroy or alter in any way any our proprietary markings on or in relation to the same.</p>
<p>13.4 You may not use any names, images and logos identifying a Platform (<strong>Marks</strong>) without prior approval. If you wish to use any of the Marks, please contact us using the details provided in clause 2.2, stating which Marks you wish to use and how and why you wish to use the Marks. Please include your name, address, telephone number, fax number and email address.</p>
<h2 class="h2 nhsuk-heading-l">14. OUR RESPONSIBILITY FOR LOSS OR DAMAGE SUFFERED BY YOU</h2>
<p>14.1 We do not exclude or limit in any way our liability to you where it would be unlawful to do so. This includes liability for death or personal injury caused by our negligence or the negligence of our employees, agents or subcontractors and for fraud or fraudulent misrepresentation.</p>
<p>14.2 We exclude all implied conditions, warranties, representations or other terms that may apply to a Platform or any Content, including but not limited to the implied warranties of satisfactory quality, fitness for a particular purpose, non-infringement, compatibility, security and accuracy.</p>
<p>14.3 To the maximum extent allowed by law, we exclude all liability arising from:</p>
<p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">14.3.1 errors or omissions in the Content;</p>
<p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">14.3.2 non-availability of access to the Content; and</p>
<p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">14.3.3 use of the Content by you in circumstances where such use is inappropriate.</p>
<p>14.4 Although we may from time to time monitor or review discussions, chat, postings, transmissions, bulletin boards and other communications media on a Platform, we are under no obligation to do so and assume no responsibility or liability arising from the Content of any such locations nor for any error, omission, infringement, defamation, obscenity, or inaccuracy contained in any information within such locations on a Platform.</p>
<p>14.5 We do not accept any responsibility for any loss, disruption or damage to your data or your computer system which may occur whilst using material derived from a Platform.</p>
<p>14.6 We assume no responsibility for any Third Party Content.</p>
<p>14.7 We neither warrant nor guarantee that:</p>
<p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">14.7.1 your use of a Platform will be uninterrupted or error-free or compatible with any third party software or equipment;</p>
<p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">14.7.2 defects in relation to a Platform, if any, will be corrected;</p>
<p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">14.7.3 a Platform and/or the information obtained by you through a Platform will meet your requirements;</p>
<p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">14.7.4 the Content (or any part of it) is up-to-date, complete, accurate, reliable, suitable, safe or will be made available at all times;</p>
<p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">14.7.5 the Content will be uninterrupted or error-free;</p>
<p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">14.7.6 that a Platform or the server that makes it available are free of viruses.</p>
<p>14.8 We will not be responsible for any delays, delivery failures, or any other loss or damage resulting from the transfer of data over communications networks and facilities, including the internet, or any failure caused by third party software and you acknowledge that a Platform may be subject to limitations, delays and other problems inherent in the use of such communications facilities.</p>
<p>14.9 In no event will we be liable to you for any loss or damage, whether in contract, tort (including negligence), breach of statutory duty, or otherwise, even if foreseeable, arising under or in connection with use of, or inability to use, a Platform or in reliance on any Content, including (but not limited) to in relation to:</p>
<p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">14.9.1 loss of profits, sales, business, or revenue;</p>
<p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">14.9.2 business interruption;</p>
<p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">14.9.3 loss of anticipated savings;</p>
<p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">14.9.4 loss of business opportunity, goodwill or reputation; or</p>
<p class="nhsuk-u-padding-left-4 nhsuk-u-padding-bottom-2 nhsuk-u-margin-0">14.9.5 any indirect or consequential loss or damage.</p>
<h2 class="h2 nhsuk-heading-l">15. INDEMNITIES</h2>
<p>15.1 You agree to indemnify and hold harmless and continue to indemnify and hold harmless us, our employees, agents, partners, sub-contractors, third party software providers and collaborators anywhere in the world in all respects and in relation to all matters relating to your use (and that of any other person acting on your authority) of a Platform, including (without limitation) in respect of any breach of these Terms to the fullest extent permitted by law.</p>
<p>15.2 Without prejudice to clause 15.1, you retain responsibility for Contributions in all respects and agree to indemnify and hold harmless and continue to indemnify us and hold us harmless in all respects and in relation to all matters in respect of Contributions, including in respect of breach of the warranties given under clause 7.2, to the fullest extent permitted by law.</p>
<h2 class="h2 nhsuk-heading-l">16. SUSPENSION AND DEACTIVATION OF YOUR ACCOUNT</h2>
<p>16.1 We reserve the right to disable or limit the use of your Account at any time without notice and without giving any reason if you have failed to comply with any of the provisions of these Terms or if we suspect any unauthorised use or misuse of your Account, any Content or a Platform generally. We reserve all rights not expressly granted by these Terms and conferred by law. In the event that your Account is disabled, your right to access a Platform shall (without prejudice to any of our rights and remedies) terminate immediately.</p>
<p>16.2 Should you wish to deactivate your Account at any time, please contact us using the contact details provided in clause 2.2. In such circumstances, we will aim to deactivate your Account within 5 (five) working days, but please note that this may take longer.</p>
<h2 class="h2 nhsuk-heading-l">17. GENERAL</h2>
<p>17.1 <strong>Transfer of these Terms</strong>: We may transfer our rights and obligations under these Terms to another organisation, including but not limited to NHS England. You may not transfer any of your rights or obligations under these Terms to another person.</p>
<p>17.2 <strong>Entire agreement</strong>: These Terms and the documents expressly referred to in them contain the whole agreement between you and us relating to its subject matter and supersedes any prior agreements, representations or understandings between them unless expressly incorporated by reference in these Terms.</p>
<p>17.3 <strong>Waiver and delay</strong>: No delay, act or omission by us in exercising any right or remedy will be deemed a waiver of that, or any other, right or remedy.</p>
<p>17.4 <strong>Severability</strong>: If any part of these Terms is declared unenforceable or invalid, the remainder will continue to be valid and enforceable.</p>
<p>17.5 <strong>Governing law and jurisdiction</strong>: These Terms, their subject matter and their formation (and any non-contractual disputes or claims) are governed by English law. We both agree to the exclusive jurisdiction of the courts of England and Wales.</p>
<p>17.6 <strong>Disclaimer</strong>: NHS England takes no responsibility for and does not endorse any content or any link to content uploaded to the platform which is created for advertising, promotional or other commercial purposes, requires a subscription or payment, promotes a business name and/or logo, or contains a link to an app via iOS or Google Play on which the user has a commercial interest.</p>'
           ,10
           ,1
           ,1
           ,0
           ,4
           ,SYSDATETIMEOFFSET());






