import { Badge } from "@/components/ui/badge";

const ProviderBadge = ({
  provider,
  codeOnly = false,
}: {
  provider?: string;
  codeOnly?: boolean;
}) => {
  const prov = provider ?? "Unknown";

  if (!codeOnly) {
    return (
      <Badge
        variant={provider ? "secondary" : "outline"}
        className="flex items-center gap-2"
        title={prov}
      >
        {provider ? <span className="font-medium">{prov}</span> : "Unknown Provider"}
      </Badge>
    );
  }

  if (codeOnly) {
    const match = prov.match(/^(.*?)(\s*\([^)]+\))\s*$/);
    const code = match ? match[2].trim() : "";

    if (!code) {
      return (
        <Badge variant="outline" className="opacity-70">
          N/A
        </Badge>
      );
    }

    return (
      <Badge
        variant={provider ? "secondary" : "outline"}
        className="flex items-center gap-2"
        title={prov}
      >
        {code ? (
          <span className="ml-2 flex-none text-xs opacity-90 whitespace-nowrap">{code}</span>
        ) : null}
      </Badge>
    );
  }
};

export default ProviderBadge;
