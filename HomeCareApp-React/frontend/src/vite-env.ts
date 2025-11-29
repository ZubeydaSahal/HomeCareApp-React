interface ImportMetaEnv {
  readonly VITE_API_URL: string;
  // legg til andre VITE_ variabler her hvis du har flere
  // readonly VITE_OTHER_THING: string;
}

interface ImportMeta {
  readonly env: ImportMetaEnv;
}
