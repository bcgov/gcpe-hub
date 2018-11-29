<%@ Page Title="" Language="C#" MasterPageFile="~/Calendar/Site.master" AutoEventWireup="True" CodeBehind="ValidationTesting.aspx.cs" Inherits="ValidationTesting" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
    <link href="Css/screen.css" rel="stylesheet" type="text/css" />

<style type="text/css">
#ctl03 { width: 500px; }
#ctl03 label { width: 250px; }
#ctl03 label.error, #ctl03 input.submit { margin-left: 253px; }
</style>


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<fieldset>
		<legend>Please provide your name, email address (won't be published) and a comment</legend>
		<p>
			<label for="cname">Name (required, at least 2 characters)</label>

			<input id="cname" name="name" class="required" minlength="2" />
		</p>
		<p>
			<label for="cemail">E-Mail (required)</label>
			<input id="cemail" name="email" class="required email" />
		</p>
		<p>
			<label for="curl">URL (optional)</label>

			<input id="curl" name="url" class="url" value="" />
		</p>
		<p>
			<label for="ccomment">Your comment (required)</label>
			<textarea id="ccomment" name="comment" class="required"></textarea>
		</p>
		<p>
			<input class="submit" type="submit" value="Submit"/>

		</p>
	</fieldset>

</asp:Content>

