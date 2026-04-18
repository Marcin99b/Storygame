import { reactRouter } from "@react-router/dev/vite";
import tailwindcss from "@tailwindcss/vite";
import { defineConfig } from "vite";

export default defineConfig({
  plugins: [tailwindcss(), reactRouter()],
  resolve: {
    tsconfigPaths: true,
  },
  server: {
    proxy: {
      "/api": {
        target: "http://localhost:5263",
        changeOrigin: true,
      },
    },
    middleware: [
      (req, res, next) => {
        if (req.url?.startsWith("/.well-known/appspecific/")) {
          res.setHeader("Content-Type", "application/json");
          res.end("{}");
          return;
        }
        next();
      },
    ],
  },
});
