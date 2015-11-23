<%@ Application Language="C#" Inherits="Microsoft.Practices.CompositeWeb.WebClientApplication" %>
<script RunAt="server">

    private static readonly string ERROR_PAGE_LOCATION = "~/Oopes.aspx";
    
    void Application_Error(object sender, EventArgs e)
    {
        if (Context != null && Context.IsCustomErrorEnabled)
        {
            Server.Transfer(ERROR_PAGE_LOCATION, false);
        }
    }
    
    
</script>