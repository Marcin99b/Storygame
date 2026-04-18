import { type RouteConfig, index, layout, route } from "@react-router/dev/routes";

export default [
  route("login", "routes/login.tsx"),
  layout("routes/_layout.tsx", [
    index("routes/library.tsx"),
    route("catalog", "routes/catalog.tsx"),
  ]),
  route("*", "routes/not-found.tsx"),
] satisfies RouteConfig;
