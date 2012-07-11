namespace System.Reflection
{
    public enum MemberInfoType 
    {
        FieldInfo,
        PropertyInfo
    }

    public static class MemberInfoExtensions
    {
        public static MemberInfoType GetMemberInfoType(this MemberInfo memberInfo)
        {
            var propertyInfo = memberInfo as PropertyInfo;

            if (propertyInfo != null)
            {
                return MemberInfoType.PropertyInfo;
            }
            else
            {
                return MemberInfoType.FieldInfo;
            }
        }
    }
}
